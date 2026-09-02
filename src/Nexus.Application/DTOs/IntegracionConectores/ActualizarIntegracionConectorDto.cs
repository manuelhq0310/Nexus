using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.IntegracionConectores;

public class ActualizarIntegracionConectorDto
{
    [Required(ErrorMessage = "La ruta del endpoint es obligatoria.")]
    [MaxLength(250)]
    public string RutaEndpoint { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ColaRabbitMQDestino { get; set; }
}
