namespace Nexus.Application.DTOs.Auth;

/// <summary>
/// Representación pública (sin datos sensibles) de un usuario.
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
