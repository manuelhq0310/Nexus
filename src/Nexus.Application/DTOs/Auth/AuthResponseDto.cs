namespace Nexus.Application.DTOs.Auth;

/// <summary>
/// Respuesta devuelta al registrar o autenticar un usuario correctamente.
/// </summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}
