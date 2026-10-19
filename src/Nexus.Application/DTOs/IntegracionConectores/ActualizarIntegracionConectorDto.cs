using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.IntegracionConectores;

public class ActualizarIntegracionConectorDto
{
    /// <summary>Obligatoria solo si la integración NO es asíncrona (se valida en el servicio).</summary>
    [MaxLength(250)]
    public string? RutaEndpoint { get; set; }

    /// <summary>Obligatoria solo si la integración es asíncrona (se valida en el servicio).</summary>
    [MaxLength(100)]
    public string? ColaRabbitMQDestino { get; set; }

    public bool EsConsultaGenerica { get; set; }

    public long? UnoEConsultaConfigId { get; set; }
}
