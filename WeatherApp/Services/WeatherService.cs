using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

/// <summary>
/// Serviço principal que faz as requisições para a API do OpenWeatherMap.
/// 
/// --- COMO FUNCIONA ---
/// 1. O usuário digita o nome de uma cidade
/// 2. Nós montamos a URL com a cidade e a chave da API
/// 3. Enviamos uma requisição HTTP GET para api.openweathermap.org
/// 4. A API retorna um JSON com todos os dados do clima
/// 5. Nós convertemos (desserializamos) esse JSON para objetos C#
/// 6. Transformamos para nosso modelo simplificado (DadosClima)
/// 7. Exibimos na tela com animações!
/// 
/// --- TRATAMENTO DE ERROS ---
/// Se a cidade não existir ou a chave for inválida, a API retorna
/// códigos de erro HTTP que nós capturamos e exibimos como mensagens
/// amigáveis em português.
/// </summary>
public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ApiKeyService _apiKeyService;

    // URL base da API do OpenWeatherMap
    // A documentação completa está em: https://openweathermap.org/current
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

    public WeatherService(HttpClient httpClient, ApiKeyService apiKeyService)
    {
        _httpClient = httpClient;
        _apiKeyService = apiKeyService;
    }

    /// <summary>
    /// Busca o clima atual de uma cidade.
    /// Este é o método principal que você vai chamar na página.
    /// 
    /// Parâmetros:
    ///   cidade - Nome da cidade (ex: "São Paulo", "London", "New York")
    /// 
    /// Retorno:
    ///   DadosClima - Objeto com todas as informações do clima
    ///   null - Se algo der errado (cidade não encontrada, erro de rede, etc)
    /// 
    /// Exceções:
    ///   Esta método NÃO lança exceções - ele captura tudo e retorna null
    ///   em caso de erro, para não quebrar a interface do usuário.
    /// </summary>
    public async Task<DadosClima?> BuscarClimaAsync(string cidade)
    {
        try
        {
            // Pega a chave da API salva no navegador
            var chave = await _apiKeyService.ObterChaveAsync();
            if (string.IsNullOrEmpty(chave))
            {
                throw new InvalidOperationException("Chave da API não configurada");
            }

            // Monta a URL completa com os parâmetros
            // units=metric -> graus Celsius (°C)
            // lang=pt_br   -> descrições em português
            // appid=       -> sua chave de acesso
            var url = $"{BaseUrl}?q={Uri.EscapeDataString(cidade)}&units=metric&lang=pt_br&appid={chave}";

            // Faz a requisição HTTP GET para a API
            var resposta = await _httpClient.GetAsync(url);

            // Se a resposta não foi bem-sucedida (ex: 404 - cidade não encontrada)
            if (!resposta.IsSuccessStatusCode)
            {
                var erroJson = await resposta.Content.ReadAsStringAsync();
                var erroObj = JsonSerializer.Deserialize<ApiErrorResponse>(erroJson);
                var mensagem = (resposta.StatusCode) switch
                {
                    System.Net.HttpStatusCode.NotFound => $"Cidade '{cidade}' não encontrada. Verifique o nome e tente novamente.",
                    System.Net.HttpStatusCode.Unauthorized => "Chave da API inválida. Verifique sua chave no OpenWeatherMap.",
                    _ => erroObj?.Message ?? "Erro ao buscar dados do clima."
                };
                throw new ApiException(mensagem, (int)resposta.StatusCode);
            }

            // Lê o JSON da resposta
            var json = await resposta.Content.ReadAsStringAsync();

            // Converte (desserializa) o JSON para nosso objeto C#
            var apiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(json);

            // Se não conseguiu converter, retorna null
            if (apiResponse == null)
                throw new ApiException("Resposta inválida da API.", 0);

            // Converte o objeto da API para nosso modelo simplificado de exibição
            return ConverterParaDadosClima(apiResponse);
        }
        catch (HttpRequestException)
        {
            // Erro de rede - sem internet, servidor fora do ar, etc
            throw new ApiException("Não foi possível conectar ao servidor. Verifique sua conexão com a internet.", 0);
        }
        catch (TaskCanceledException)
        {
            // Timeout - a demorou muito para responder
            throw new ApiException("A requisição excedeu o tempo limite. Tente novamente.", 0);
        }
        catch (JsonException)
        {
            // Erro ao processar o JSON - resposta inesperada da API
            throw new ApiException("Erro ao processar os dados recebidos. Tente novamente.", 0);
        }
    }

    /// <summary>
    /// Converte a resposta completa da API (WeatherApiResponse) para nosso
    /// modelo simplificado (DadosClima) que é mais fácil de usar na interface.
    /// 
    /// Isso é uma boa prática: separar o modelo da API externa do modelo
    /// que usamos na nossa interface. Se a API mudar, só precisamos
    /// alterar este método de conversão!
    /// </summary>
    private static DadosClima ConverterParaDadosClima(WeatherApiResponse apiResponse)
    {
        var clima = apiResponse.Weather?.FirstOrDefault();
        var agora = DateTimeOffset.FromUnixTimeSeconds(apiResponse.Dt).DateTime;

        return new DadosClima
        {
            Cidade = apiResponse.Nome,
            Pais = apiResponse.Sys?.Pais ?? "",
            Temperatura = apiResponse.Main?.Temperatura ?? 0,
            SensacaoTermica = apiResponse.Main?.SensacaoTermica ?? 0,
            TempMinima = apiResponse.Main?.TempMinima ?? 0,
            TempMaxima = apiResponse.Main?.TempMaxima ?? 0,
            Umidade = apiResponse.Main?.Umidade ?? 0,
            Pressao = apiResponse.Main?.Pressao ?? 0,
            VelocidadeVento = apiResponse.Wind?.Velocidade ?? 0,
            DirecaoVento = apiResponse.Wind?.Direcao ?? 0,
            Descricao = clima?.Descricao ?? "",
            Icone = clima?.Icone ?? "01d",
            CondicaoPrincipal = clima?.Principal ?? "",
            Visibilidade = apiResponse.Visibilidade,
            CoberturaNuvens = apiResponse.Clouds?.Todos ?? 0,
            DataHora = agora,
            NascerDoSol = DateTimeOffset.FromUnixTimeSeconds(apiResponse.Sys?.NascerDoSol ?? 0).DateTime,
            PorDoSol = DateTimeOffset.FromUnixTimeSeconds(apiResponse.Sys?.PorDoSol ?? 0).DateTime,
            Latitude = apiResponse.Coord?.Lat ?? 0,
            Longitude = apiResponse.Coord?.Lon ?? 0,
            EhDia = clima?.Icone?.EndsWith("d") ?? true
        };
    }
}

/// <summary>
/// Modelo para desserializar respostas de erro da API.
/// </summary>
public class ApiErrorResponse
{
    public int Cod { get; set; }
    public string Message { get; set; } = "";
}

/// <summary>
/// Exceção personalizada para erros da API do clima.
/// Contém uma mensagem amigável em português e o código HTTP.
/// </summary>
public class ApiException : Exception
{
    public int StatusCode { get; }

    public ApiException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
