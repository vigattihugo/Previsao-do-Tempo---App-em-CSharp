# 🌤️ Previsão do Tempo - App em C# com Blazor WebAssembly

**Aprendendo C# construindo um aplicativo de clima completo do zero!**

Este projeto foi criado especialmente para quem está **começando a aprender C#**. Cada decisão de código é explicada abaixo para que você entenda não apenas **o que** o código faz, mas **por que** ele foi escrito dessa forma.

---

## 🚀 Como Executar

### 1. Pré-requisitos
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) instalado
- Uma chave grátis da [OpenWeatherMap](https://openweathermap.org/appid)

### 2. Executar o projeto
```bash
cd WeatherApp
dotnet run
```

### 3. Configurar a chave da API
1. Ao abrir o app, clique em **"Configurar Chave da API"**
2. Siga as instruções para criar uma conta grátis no OpenWeatherMap
3. Cole sua chave e clique em **Salvar**

> 💡 A chave fica salva no seu navegador (localStorage) — você só precisa configurar uma vez!

---

## 📁 Estrutura do Projeto

```
WeatherApp/
├── Models/
│   └── WeatherData.cs          # 📦 Modelos de dados (o "formato" das informações)
├── Services/
│   ├── ApiKeyService.cs         # 🔑 Gerencia a chave da API
│   └── WeatherService.cs        # 🌐 Faz requisições para a API
├── Pages/
│   ├── Weather.razor            # 🏠 Página principal do clima
│   └── Animations/
│       ├── WeatherSunny.razor   # ☀️ Animação: Ensolarado
│       ├── WeatherNight.razor   # 🌙 Animação: Noite
│       ├── WeatherCloudy.razor  # ☁️ Animação: Nublado
│       ├── WeatherRainy.razor   # 🌧️ Animação: Chuvoso
│       ├── WeatherStormy.razor  # ⛈️ Animação: Tempestade
│       ├── WeatherSnowy.razor   # ❄️ Animação: Nevando
│       └── WeatherFoggy.razor   # 🌫️ Animação: Nevoeiro
├── Layout/
│   └── MainLayout.razor         # 🎨 Layout principal (cabeçalho + rodapé)
├── wwwroot/
│   ├── css/
│   │   └── app.css              # 🎭 Animações e estilos
│   └── index.html               # 📄 Página HTML inicial
├── Program.cs                   # 🚀 Ponto de entrada do aplicativo
└── README.md                    # 📖 Este arquivo (aprendendo C#!)
```

---

## 📚 Lições de C# - Cada Arquivo Explicado

### 1️⃣ `Program.cs` - O Ponto de Partida 🚀

**O que é:** O arquivo que o .NET executa primeiro. É como a "porta de entrada" do seu aplicativo.

```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);
```

**Por que assim:** Todo aplicativo Blazor WebAssembly precisa de um `WebAssemblyHostBuilder`. Pense nele como um "construtor de casa" que prepara tudo antes de entregar a chave.

```csharp
builder.Services.AddScoped<WeatherService>();
```

**Por que assim:** Isso é **Injeção de Dependência (DI)**. Em vez de criar um `new WeatherService()` manualmente em cada página, nós **registramos** o serviço aqui e o Blazor **injeta** ele automaticamente onde precisarmos. Isso é um padrão profissional que:
- ✅ Facilita testes
- ✅ Evita repetir código
- ✅ Gerencia a memória automaticamente

**Tipos de registro:**
| Tipo | Duração | Quando usar |
|------|---------|-------------|
| `AddSingleton` | Uma vez para todo o app | Configurações globais |
| `AddScoped` | Uma vez por usuário | Serviços web (recomendado) |
| `AddTransient` | Toda vez que for usado | Serviços leves e sem estado |

---

### 2️⃣ `Models/WeatherData.cs` - O "Formulário" dos Dados 📋

**O que é:** Define a **estrutura** dos dados que recebemos da API. É como um formulário em branco: você sabe exatamente onde cada informação vai se encaixar.

```csharp
public class WeatherApiResponse
{
    [JsonPropertyName("coord")]
    public Coordenadas? Coord { get; set; }
    // ...
}
```

**Por que assim:** A API do OpenWeatherMap retorna um JSON (JavaScript Object Notation) — um formato de texto estruturado. O C# precisa saber "onde" cada campo está no JSON para poder preencher nossos objetos. O `[JsonPropertyName]` diz: *"O campo 'coord' no JSON vai para a propriedade 'Coord' no C#"*.

**Por que usar `?` (nullable):** `Coordenadas?` significa que este campo pode ser `null` (vazio). Nem sempre a API retorna todos os campos, então usamos `?` para evitar erros.

**Por que duas classes (WeatherApiResponse e DadosClima):** Separação de responsabilidades!
- `WeatherApiResponse`: Espelha EXATAMENTE o JSON da API (pode mudar se a API mudar)
- `DadosClima`: É nosso modelo **interno** (mais simples, só com o que precisamos)

Isso é o **Princípio da Segregação de Interfaces**: não dependa de coisas que você não usa.

---

### 3️⃣ `Services/WeatherService.cs` - O "Carteiro" 🌐

**O que é:** A classe que **busca** os dados do clima na internet. É como um carteiro que vai até a API, pega o pacote (JSON) e entrega para você já traduzido.

```csharp
public async Task<DadosClima?> BuscarClimaAsync(string cidade)
{
    try
    {
        var resposta = await _httpClient.GetAsync(url);
        // ...
    }
    catch (HttpRequestException)
    {
        throw new ApiException("Não foi possível conectar ao servidor.");
    }
}
```

**Por que `async` e `await`:** Requisições de internet são **lentas** (levam milissegundos a segundos). Sem `async/await`, o programa TRAVARIA esperando a resposta. Com `async`, ele diz: *"Vou esperar, mas enquanto isso, deixe a interface do usuário funcionar"*.

```csharp
var url = $"{BaseUrl}?q={Uri.EscapeDataString(cidade)}&units=metric&lang=pt_br&appid={chave}";
```

**Por que `Uri.EscapeDataString`:** Se o usuário digitar "São Paulo", o espaço precisa ser convertido para `%20` na URL. O `EscapeDataString` faz isso automaticamente!

**Por que tratamento de erro em camadas:**
```csharp
try { /* código que pode falhar */ }
catch (HttpRequestException) { /* erro de rede */ }
catch (TaskCanceledException) { /* timeout */ }
catch (JsonException) { /* JSON inválido */ }
```

Cada tipo de exceção representa um problema diferente, e tratamos cada um com uma mensagem específica em português. O usuário não precisa ver "System.Net.Http.HttpRequestException" — ele vê "Verifique sua conexão com a internet"!

---

### 4️⃣ `Pages/Weather.razor` - A Interface do Usuário 🖥️

**O que é:** A página principal que o usuário vê. Combina HTML com C# usando **Razor** (a sintaxe de templates do Blazor).

```razor
@page "/"
@inject WeatherService WeatherService
```

**O que faz:**
- `@page "/"` → Diz que esta página é a **raiz** do site (home)
- `@inject WeatherService` → Pega o serviço registrado em `Program.cs` (Injeção de Dependência)

#### Variáveis de Estado
```csharp
private string _cidadeInput = "";
private DadosClima? _dadosClima;
private bool _carregando;
```

**Por que `_` no nome:** Convenção C#: variáveis privadas começam com `_` underline. Isso ajuda a diferenciar de variáveis públicas e parâmetros.

#### Ciclo de Vida
```csharp
protected override async Task OnInitializedAsync()
{
    _apiKey = await ApiKeyService.ObterChaveAsync() ?? "";
}
```

**OnInitializedAsync:** É executado automaticamente quando a página carrega. É o lugar ideal para carregar dados iniciais.

#### Tratamento de Erros na UI
```csharp
catch (ApiException ex)
{
    _mensagemErro = ex.Message; // Mensagem amigável em PT-BR
}
```

Mostramos uma mensagem bonita na tela em vez de deixar o aplicativo quebrar.

#### Auto-Refresh (Atualização Automática)
```csharp
private async Task ExecutarAutoRefreshAsync(CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        await Task.Delay(1000, token);
        // ... atualiza a cada 60 segundos
    }
}
```

**Por que `CancellationToken`:** Permite **cancelar** o loop de forma segura quando o usuário desliga o auto-refresh. Sem ele, teríamos um loop infinito!

---

### 5️⃣ Animações CSS e Componentes 🎭

**O que é:** Cada condição climática tem seu próprio componente Blazor que renderiza uma animação em CSS puro.

**Por que CSS puro (sem JavaScript):**
- ✅ Mais rápido (não precisa carregar bibliotecas JS)
- ✅ Mais simples de entender
- ✅ Funciona em qualquer navegador moderno
- ✅ Consome menos recursos

**Exemplo: Animação de Chuva 🌧️**
```css
.raindrop {
    animation: rain 0.8s linear infinite;
}

@keyframes rain {
    0% { transform: translateY(-20px); opacity: 0; }
    50% { opacity: 1; }
    100% { transform: translateY(200px); opacity: 0; }
}
```

**Como funciona:** A animação `rain` move a gota de cima para baixo em 0.8 segundos e repete infinitamente. As gotas têm `animation-delay` diferentes (calculados no C#) para não caírem todas juntas!

**Por que componentes separados para cada clima?**
- Cada componente é pequeno e focado
- Fácil de modificar um clima sem afetar os outros
- Reutilizável em outras páginas

---

### 6️⃣ `wwwroot/css/app.css` - Onde a Mágica Acontece ✨

**O que é:** Arquivo com todos os estilos e animações.

**Tema escuro (dark mode) por padrão:**
```css
body {
    background: linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%);
}
```

**Por que tema escuro:** Aplicativos de clima costumam usar temas escuros porque:
- ✅ Parecem mais modernos ("dark mode" está na moda)
- ✅ Economizam bateria em telas OLED
- ✅ Destacam melhor as cores das animações

**Design responsivo:**
```css
@media (max-width: 480px) {
    .weather-details-grid {
        grid-template-columns: repeat(2, 1fr);
    }
}
```

O layout se adapta automaticamente a celulares, tablets e desktops!

---

## 🎯 Funcionalidades Implementadas

| Funcionalidade | Status | Como funciona |
|---------------|--------|---------------|
| 🌐 Buscar clima por cidade | ✅ | Digita o nome, pesquisa com Enter ou botão |
| 📊 Exibir temperatura, umidade, vento | ✅ | Grid com 8 cards de informações |
| 🔍 Sugestões de cidades | ✅ | Botões "São Paulo", "Rio", "London"... |
| ❌ Tratamento de erros | ✅ | Mensagens em PT-BR para cada erro |
| ☀️ Animações climáticas | ✅ | 7 animações CSS diferentes |
| 🔄 Auto-refresh a cada 60s | ✅ | Botão liga/desliga com timer |
| 💾 Chave da API persistente | ✅ | Salva no localStorage do navegador |
| 📱 Design responsivo | ✅ | Funciona em celular, tablet e desktop |
| 🎨 Tema escuro | ✅ | Gradiente azul escuro profissional |

---

## 🔍 Como cada API Key funciona

1. Você cria uma conta grátis em [OpenWeatherMap](https://openweathermap.org/appid)
2. Gera uma chave de API (uma string tipo `1a2b3c4d5e6f7g8h9i0j...`)
3. O app guarda essa chave no **localStorage** do navegador
4. Quando você pesquisa uma cidade, o app monta a URL:
   ```
   https://api.openweathermap.org/data/2.5/weather?q=São Paulo&units=metric&lang=pt_br&appid=SUA_CHAVE
   ```
5. A API retorna JSON, que convertemos para objetos C#

> 🔒 **Segurança:** A chave fica no navegador (cliente). Em produção, você usaria um backend para esconder a chave. Para aprendizado, está ótimo!

---

## 🧠 Conceitos de C# que você aprendeu

| Conceito | Onde foi usado | Explicação rápida |
|----------|---------------|-------------------|
| **Classes e objetos** | `WeatherData.cs` | Blueprint para criar objetos com dados |
| **Propriedades** | `{ get; set; }` | Campos públicos de uma classe |
| **Async/Await** | `WeatherService.cs` | Programação assíncrona sem travar a UI |
| **Tratamento de exceções** | `try/catch` | Capturar e tratar erros graciosamente |
| **Injeção de Dependência** | `Program.cs` | Framework fornece dependências automaticamente |
| **Tipos nullable** | `string?` | Variáveis que podem ser null |
| **Enumeradores (enum)** | `TipoClima` | Conjunto fixo de valores nomeados |
| **JSON Serialization** | `[JsonPropertyName]` | Converter JSON para objetos C# |
| **LINQ** | `apiResponse.Weather?.FirstOrDefault()` | Consultar coleções de dados |
| **Interpolação de strings** | `$"{variavel}"` | Inserir variáveis em strings |
| **Switch expression** | `tipo switch { ... }` | Switch case moderno e conciso |
| **Componentes Blazor** | `*.razor` | Pedaços reutilizáveis de UI |
| **CSS Animations** | `@keyframes` | Animações sem JavaScript |
| **Media Queries** | `@media` | Design responsivo |

---

## 🤔 Perguntas Frequentes

### Por que Blazor WebAssembly e não MVC ou Razor Pages?
Blazor WebAssembly permite escrever C# que roda **no navegador**. É o futuro do .NET para desenvolvimento web e é muito mais divertido para aprender porque você vê os resultados imediatamente!

### Por que CSS e não JavaScript para as animações?
CSS é mais simples, mais rápido e não requer bibliotecas adicionais. Tudo que você vê (sol pulsando, nuvens se movendo, chuva caindo) é CSS puro!

### Por que separar em Models e Services?
Separar responsabilidades é uma das regras mais importantes em programação profissional. Cada arquivo tem um propósito único e claro. Isso facilita encontrar bugs, adicionar funcionalidades e testar o código.

### Por que tratamento de erro é tão importante?
Seu aplicativo VAI falhar em algum momento (internet cai, API fica offline, usuário digita "cidade XYZ" que não existe). Tratar erros faz a diferença entre um aplicativo que "quebra" e um que mostra "Cidade não encontrada. Verifique o nome."

---

## 📚 Próximos Passos para Aprender C#

1. **Adicione uma previsão para 5 dias** (use a API de forecast do OpenWeatherMap)
2. **Adicione um gráfico de temperatura** (use Chart.js ou Plotly)
3. **Salve cidades favoritas** (use localStorage)
4. **Adicione detecção automática de localização** (use Geolocation API)
5. **Publique online** (GitHub Pages, Netlify, Vercel)

---

## 📝 Licença

Este projeto é **open source** e foi criado para fins educacionais. Sinta-se à vontade para usar, modificar e compartilhar!

---

**Feito com 💜 e muito C# • Blazor WebAssembly • .NET 8**

*"O melhor jeito de aprender a programar é programando. Não tenha medo de quebrar as coisas — cada erro é uma lição!"*
