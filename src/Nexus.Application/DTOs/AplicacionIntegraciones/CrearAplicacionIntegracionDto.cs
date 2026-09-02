using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.AplicacionIntegraciones;

public class CrearAplicacionIntegracionDto
{
    [Required(ErrorMessage = "La aplicación es obligatoria.")]
    public long AplicacionId { get; set; }

    [Required(ErrorMessage = "La integración es obligatoria.")]
    public long IntegracionId { get; set; }
}
