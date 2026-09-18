using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Nexus.Presentation.Services;

namespace Nexus.Presentation.Services.Auth;

/// <summary>
/// DelegatingHandler que:
/// 1) Agrega automáticamente el header "Authorization: Bearer {token}" a toda petición
///    saliente del HttpClient de la API, si hay un token válido almacenado.
/// 2) Detecta respuestas 401 de endpoints protegidos (token vencido, revocado, o inválido)
///    y fuerza el cierre de sesión + redirección a /login. Sin esto, si el token expira
///    mientras el usuario ya está usando la app (sin volver a navegar entre páginas), las
///    llamadas a la API empiezan a fallar con 401 pero la UI se queda como si nada, porque
///    Blazor solo reevalúa el estado de autenticación al navegar entre rutas.
/// </summary>
public class JwtAuthorizationMessageHandler : DelegatingHandler
{
    private const string TokenKey = "nexus_token";
    private const string ExpiresAtKey = "nexus_token_expires_at";
    private const string UserKey = "nexus_user";

    private readonly LocalStorageService _localStorage;
    private readonly NavigationManager _navigation;
    private readonly CustomAuthStateProvider _authStateProvider;

    public JwtAuthorizationMessageHandler(
        LocalStorageService localStorage,
        NavigationManager navigation,
        Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authStateProvider)
    {
        _localStorage = localStorage;
        _navigation = navigation;
        _authStateProvider = (CustomAuthStateProvider)authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync(TokenKey);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized && !EsPeticionDeAutenticacion(request))
        {
            await ForzarCierreDeSesionAsync();
        }

        return response;
    }

    /// <summary>
    /// Un 401 al intentar iniciar sesión o registrarse significa "credenciales inválidas"
    /// (un error normal que la propia pantalla de login ya maneja y muestra), no una sesión
    /// vencida. Ese caso NO debe disparar el cierre de sesión/redirección automática.
    /// </summary>
    private static bool EsPeticionDeAutenticacion(HttpRequestMessage request)
    {
        var path = request.RequestUri?.AbsolutePath ?? string.Empty;
        return path.Contains("/api/Auth/login", StringComparison.OrdinalIgnoreCase)
            || path.Contains("/api/Auth/register", StringComparison.OrdinalIgnoreCase);
    }

    private async Task ForzarCierreDeSesionAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        await _localStorage.RemoveItemAsync(ExpiresAtKey);
        await _localStorage.RemoveItemAsync(UserKey);

        _authStateProvider.NotifyStateChanged();

        // No se navega si el usuario ya está en /login (evita un bucle de redirección
        // cuando varias peticiones en paralelo reciben 401 al mismo tiempo).
        if (!_navigation.Uri.Contains("/login", StringComparison.OrdinalIgnoreCase))
        {
            _navigation.NavigateTo("/login?sesionVencida=true", forceLoad: false);
        }
    }
}
