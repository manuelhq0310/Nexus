namespace Nexus.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa un usuario del sistema Nexus.
/// </summary>
public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash de la contraseña generado con PBKDF2 (nunca se almacena en texto plano).
    /// Formato: {iteraciones}.{salt en base64}.{hash en base64}
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }
}
