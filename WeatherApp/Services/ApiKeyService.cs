using Microsoft.JSInterop;

namespace WeatherApp.Services;

/// <summary>
/// Serviço responsável por gerenciar a chave da API (API Key) no navegador.
/// 
/// --- COMO FUNCIONA ---
/// O Blazor WebAssembly roda inteiramente no navegador do usuário.
/// Para não precisar digitar a chave toda vez, guardamos ela no
/// localStorage do navegador (um "banco de dados" simples que o
/// navegador mantém para cada site).
/// 
/// O IJSRuntime é a ponte que permite o C# chamar funções JavaScript.
/// Aqui usamos ele para ler e escrever no localStorage.
/// </summary>
public class ApiKeyService
{
    private readonly IJSRuntime _jsRuntime;
    // Nome fixo da chave no localStorage do navegador
    // NÃO é a chave da API! É apenas o identificador onde salvamos a chave.
    private const string StorageKey = "openweathermap_api_key";

    public ApiKeyService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Salva a chave da API no navegador (localStorage).
    /// </summary>
    public async Task SalvarChaveAsync(string chave)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, chave);
    }

    /// <summary>
    /// Recupera a chave da API salva no navegador.
    /// Retorna null se não houver chave salva.
    /// </summary>
    public async Task<string?> ObterChaveAsync()
    {
        return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
    }

    /// <summary>
    /// Remove a chave da API salva no navegador.
    /// </summary>
    public async Task RemoverChaveAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
    }
}
