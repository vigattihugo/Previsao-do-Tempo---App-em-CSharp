using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WeatherApp;
using WeatherApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ─── REGISTRO DOS SERVIÇOS ───
// Injeção de Dependência (DI): o Blazor cria automaticamente
// as instâncias desses serviços e as disponibiliza para as páginas.
//
// AddSingleton  → Uma única instância para todo o aplicativo
// AddScoped     → Uma instância por sessão do usuário
// AddTransient  → Uma nova instância a cada vez que é solicitado

// HttpClient é o serviço padrão para fazer requisições HTTP
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// ApiKeyService gerencia a chave da API no localStorage
builder.Services.AddScoped<ApiKeyService>();

// WeatherService é o serviço principal que busca dados do clima
builder.Services.AddScoped<WeatherService>();

await builder.Build().RunAsync();
