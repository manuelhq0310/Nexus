using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.Aplicaciones;

public class CrearAplicacionDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código de aplicación es obligatorio.")]
    public string CodigoApp { get; set; } = string.Empty;

    public string? Descripcion { get; set; }
}
