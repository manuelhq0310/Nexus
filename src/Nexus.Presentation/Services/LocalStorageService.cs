using Microsoft.JSInterop;

namespace Nexus.Presentation.Services;

/// <summary>
/// Wrapper minimalista sobre window.localStorage vía interop de JavaScript.
/// Se evita depender de paquetes externos adicionales para esta necesidad puntual.
/// </summary>
public class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask<string?> GetItemAsync(string key)
        => _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);

    public ValueTask SetItemAsync(string key, string value)
        => _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);

    public ValueTask RemoveItemAsync(string key)
        => _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
}
