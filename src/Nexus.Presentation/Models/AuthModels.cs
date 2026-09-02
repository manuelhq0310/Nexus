namespace Nexus.Presentation.Models;

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequestDto
{
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El nombre completo es obligatorio.")]
    public string FullName { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El correo es obligatorio.")]
    [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Correo inválido.")]
    public string Email { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "La contraseña es obligatoria.")]
    [System.ComponentModel.DataAnnotations.MinLength(8, ErrorMessage = "Debe tener al menos 8 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Debes confirmar la contraseña.")]
    [System.ComponentModel.DataAnnotations.Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = new();
}

/// <summary>
/// Forma del error devuelto por el middleware global de excepciones del backend Nexus.
/// </summary>
public class ApiProblemDetails
{
    public int Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TraceId { get; set; }
    public string? Detail { get; set; }
}
