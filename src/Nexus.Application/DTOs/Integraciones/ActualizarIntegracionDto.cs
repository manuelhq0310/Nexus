using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.Integraciones;

public class ActualizarIntegracionDto
{
    [Required(ErrorMessage = "El código de acción es obligatorio.")]
    [MaxLength(50)]
    public string CodigoAccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Descripcion { get; set; }
}
