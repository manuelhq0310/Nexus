using System.Net.Http.Headers;
using Nexus.Presentation.Services;

namespace Nexus.Presentation.Services.Auth;

/// <summary>
/// DelegatingHandler que agrega automáticamente el header "Authorization: Bearer {token}"
/// a toda petición saliente del HttpClient de la API, si hay un token válido almacenado.
/// </summary>
public class JwtAuthorizationMessageHandler : DelegatingHandler
{
    private const string TokenKey = "nexus_token";
    private readonly LocalStorageService _localStorage;

    public JwtAuthorizationMessageHandler(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
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

        return await base.SendAsync(request, cancellationToken);
    }
}
