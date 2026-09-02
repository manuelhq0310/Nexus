using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Nexus.Presentation.Models;
using Nexus.Presentation.Services;

namespace Nexus.Presentation.Services.Auth;

/// <summary>
/// AuthenticationStateProvider basado en el JWT guardado en localStorage.
/// No decodifica el token para leer claims "de verdad" (no es necesario aquí);
/// arma el ClaimsPrincipal a partir de los datos del UserDto que ya recibimos
/// en la respuesta de login/register, guardados junto al token.
/// </summary>
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private const string TokenKey = "nexus_token";
    private const string ExpiresAtKey = "nexus_token_expires_at";
    private const string UserKey = "nexus_user";

    private readonly LocalStorageService _localStorage;
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    public CustomAuthStateProvider(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync(TokenKey);
        var expiresAtRaw = await _localStorage.GetItemAsync(ExpiresAtKey);

        if (string.IsNullOrWhiteSpace(token) ||
            string.IsNullOrWhiteSpace(expiresAtRaw) ||
            !DateTime.TryParse(expiresAtRaw, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiresAt) ||
            expiresAt <= DateTime.UtcNow)
        {
            return new AuthenticationState(Anonymous);
        }

        var userJson = await _localStorage.GetItemAsync(UserKey);
        if (string.IsNullOrWhiteSpace(userJson))
        {
            return new AuthenticationState(Anonymous);
        }

        var user = System.Text.Json.JsonSerializer.Deserialize<UserDto>(userJson, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (user is null)
        {
            return new AuthenticationState(Anonymous);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, authenticationType: "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    /// <summary>Notifica al framework que el estado de autenticación cambió (login/logout).</summary>
    public void NotifyStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
