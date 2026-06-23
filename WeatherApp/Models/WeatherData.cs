using System.Text.Json.Serialization;

namespace WeatherApp.Models;

/// <summary>
/// Modelo principal que representa a resposta completa da API OpenWeatherMap.
/// </summary>
public class WeatherApiResponse
{
    [JsonPropertyName("coord")]
    public Coordenadas? Coord { get; set; }

    [JsonPropertyName("weather")]
    public List<Clima>? Weather { get; set; }

    [JsonPropertyName("main")]
    public DadosPrincipais? Main { get; set; }

    [JsonPropertyName("visibility")]
    public int Visibilidade { get; set; }

    [JsonPropertyName("wind")]
    public Vento? Wind { get; set; }

    [JsonPropertyName("clouds")]
    public Nuvens? Clouds { get; set; }

    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("sys")]
    public Sistema? Sys { get; set; }

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("cod")]
    public int Cod { get; set; }
}

/// <summary>
/// Coordenadas geográficas (latitude e longitude).
/// </summary>
public class Coordenadas
{
    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    [JsonPropertyName("lat")]
    public double Lat { get; set; }
}

/// <summary>
/// Dados principais de temperatura e sensação térmica.
/// </summary>
public class DadosPrincipais
{
    [JsonPropertyName("temp")]
    public double Temperatura { get; set; }

    [JsonPropertyName("feels_like")]
    public double SensacaoTermica { get; set; }

    [JsonPropertyName("temp_min")]
    public double TempMinima { get; set; }

    [JsonPropertyName("temp_max")]
    public double TempMaxima { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressao { get; set; }

    [JsonPropertyName("humidity")]
    public int Umidade { get; set; }
}

/// <summary>
/// Informações do clima (descrição, ícone, etc).
/// </summary>
public class Clima
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("main")]
    public string Principal { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Descricao { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icone { get; set; } = string.Empty;
}

/// <summary>
/// Dados do vento (velocidade e direção).
/// </summary>
public class Vento
{
    [JsonPropertyName("speed")]
    public double Velocidade { get; set; }

    [JsonPropertyName("deg")]
    public int Direcao { get; set; }
}

/// <summary>
/// Dados de cobertura de nuvens.
/// </summary>
public class Nuvens
{
    [JsonPropertyName("all")]
    public int Todos { get; set; }
}

/// <summary>
/// Informações do nascer/pôr do sol e país.
/// </summary>
public class Sistema
{
    [JsonPropertyName("country")]
    public string Pais { get; set; } = string.Empty;

    [JsonPropertyName("sunrise")]
    public long NascerDoSol { get; set; }

    [JsonPropertyName("sunset")]
    public long PorDoSol { get; set; }
}

/// <summary>
/// Modelo simplificado para exibição na interface do usuário.
/// </summary>
public class DadosClima
{
    public string Cidade { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public double Temperatura { get; set; }
    public double SensacaoTermica { get; set; }
    public double TempMinima { get; set; }
    public double TempMaxima { get; set; }
    public int Umidade { get; set; }
    public int Pressao { get; set; }
    public double VelocidadeVento { get; set; }
    public int DirecaoVento { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Icone { get; set; } = string.Empty;
    public string CondicaoPrincipal { get; set; } = string.Empty;
    public int Visibilidade { get; set; }
    public int CoberturaNuvens { get; set; }
    public DateTime DataHora { get; set; }
    public DateTime NascerDoSol { get; set; }
    public DateTime PorDoSol { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool EhDia { get; set; }

    /// <summary>
    /// Retorna o tipo de condição climática para animações.
    /// </summary>
    public TipoClima TipoClima
    {
        get
        {
            int id = int.TryParse(Icone?.Substring(0, 2), out var result) ? result * 10 : 0;

            return CondicaoPrincipal.ToLower() switch
            {
                "clear" => EhDia ? TipoClima.ClearDay : TipoClima.ClearNight,
                "clouds" => TipoClima.Cloudy,
                "rain" or "drizzle" => TipoClima.Rainy,
                "thunderstorm" => TipoClima.Stormy,
                "snow" => TipoClima.Snowy,
                "mist" or "fog" or "haze" => TipoClima.Foggy,
                _ => TipoClima.Cloudy
            };
        }
    }

    /// <summary>
    /// Converte graus para texto da direção do vento.
    /// </summary>
    public string DirecaoVentoTexto
    {
        get
        {
            string[] direcoes = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE",
                                  "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
            int index = (int)Math.Round(DirecaoVento / 22.5) % 16;
            return direcoes[index];
        }
    }
}

/// <summary>
/// Enumeração dos tipos de clima para as animações.
/// </summary>
public enum TipoClima
{
    ClearDay,   // Dia ensolarado ☀️
    ClearNight, // Noite estrelada 🌙
    Cloudy,     // Nublado ☁️
    Rainy,      // Chuvoso 🌧️
    Stormy,     // Tempestade ⛈️
    Snowy,      // Nevando ❄️
    Foggy       // Nevoeiro 🌫️
}
