using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.Auth;

/// <summary>
/// Credenciales requeridas para autenticar un usuario.
/// </summary>
public class LoginRequestDto
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = string.Empty;
}
