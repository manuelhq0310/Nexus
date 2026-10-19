using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.IntegracionConectores;

public class CrearIntegracionConectorDto
{
    [Required(ErrorMessage = "La integración es obligatoria.")]
    public long IntegracionId { get; set; }

    [Required(ErrorMessage = "El conector es obligatorio.")]
    public long ConectorId { get; set; }

    /// <summary>Obligatoria solo si la integración NO es asíncrona (se valida en el servicio).</summary>
    [MaxLength(250)]
    public string? RutaEndpoint { get; set; }

    /// <summary>Obligatoria solo si la integración es asíncrona (se valida en el servicio).</summary>
    [MaxLength(100)]
    public string? ColaRabbitMQDestino { get; set; }

    /// <summary>Solo aplica a integraciones de tipo Consulta.</summary>
    public bool EsConsultaGenerica { get; set; }

    /// <summary>Consulta UnoE asociada; obligatoria cuando EsConsultaGenerica = true.</summary>
    public long? UnoEConsultaConfigId { get; set; }
}
