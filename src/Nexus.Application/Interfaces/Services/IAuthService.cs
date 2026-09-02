using Nexus.Application.DTOs.Auth;

namespace Nexus.Application.Interfaces.Services;

/// <summary>
/// Servicio de aplicación que orquesta el registro y autenticación de usuarios.
/// </summary>
public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
