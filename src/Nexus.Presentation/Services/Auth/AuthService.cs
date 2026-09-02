using System.Net.Http.Json;
using System.Text.Json;
using Nexus.Presentation.Models;
using Nexus.Presentation.Services;

namespace Nexus.Presentation.Services.Auth;

/// <summary>
/// Orquesta login/registro/logout: llama a la API, persiste el token en localStorage
/// y notifica al CustomAuthStateProvider para refrescar el estado de autenticación de la UI.
/// </summary>
public class AuthService
{
    private const string TokenKey = "nexus_token";
    private const string ExpiresAtKey = "nexus_token_expires_at";
    private const string UserKey = "nexus_user";

    private readonly HttpClient _httpClient;
    private readonly LocalStorageService _localStorage;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthService(
        HttpClient httpClient,
        LocalStorageService localStorage,
        Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = (CustomAuthStateProvider)authStateProvider;
    }

    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", new LoginRequestDto
        {
            Email = email,
            Password = password
        });

        return await ProcesarRespuestaAutenticacionAsync(response);
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequestDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", dto);
        return await ProcesarRespuestaAutenticacionAsync(response);
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        await _localStorage.RemoveItemAsync(ExpiresAtKey);
        await _localStorage.RemoveItemAsync(UserKey);
        _authStateProvider.NotifyStateChanged();
    }

    private async Task<(bool Success, string? Error)> ProcesarRespuestaAutenticacionAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var mensaje = await LeerMensajeErrorAsync(response);
            return (false, mensaje);
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (auth is null)
        {
            return (false, "La respuesta del servidor no tuvo el formato esperado.");
        }

        await _localStorage.SetItemAsync(TokenKey, auth.Token);
        await _localStorage.SetItemAsync(ExpiresAtKey, auth.ExpiresAt.ToString("o"));
        await _localStorage.SetItemAsync(UserKey, JsonSerializer.Serialize(auth.User));

        _authStateProvider.NotifyStateChanged();
        return (true, null);
    }

    private static async Task<string> LeerMensajeErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
            if (problem is not null && !string.IsNullOrWhiteSpace(problem.Title))
            {
                return problem.Title;
            }
        }
        catch
        {
            // Ignorado: si el cuerpo no es JSON válido, se usa el mensaje genérico de abajo.
        }

        return $"Ocurrió un error ({(int)response.StatusCode}). Intenta nuevamente.";
    }
}
