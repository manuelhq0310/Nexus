using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.IntegracionConectores;

public class CrearIntegracionConectorDto
{
    [Required(ErrorMessage = "La integración es obligatoria.")]
    public long IntegracionId { get; set; }

    [Required(ErrorMessage = "El conector es obligatorio.")]
    public long ConectorId { get; set; }

    [Required(ErrorMessage = "La ruta del endpoint es obligatoria.")]
    [MaxLength(250)]
    public string RutaEndpoint { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ColaRabbitMQDestino { get; set; }
}
