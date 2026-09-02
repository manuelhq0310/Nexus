namespace Nexus.Domain.Entities.Integraciones;

/// <summary>
/// Tabla intermedia que define qué <see cref="IntgConector"/> resuelve una
/// <see cref="IntgIntegracion"/> específica, registrando el path/endpoint
/// complementario a la UrlBase del conector.
/// </summary>
public class IntgIntegracionConector : IntgBaseEntity
{
    public long IntegracionId { get; set; }
    public long ConectorId { get; set; }

    /// <summary>Ej: "api/v1/Anticipos/RegistrarAnticipo" (se concatena con UrlBase).</summary>
    public string RutaEndpoint { get; set; } = string.Empty;

    /// <summary>Ej: "sapecc-anticipos-queue".</summary>
    public string? ColaRabbitMQDestino { get; set; }

    // Navegación
    public IntgIntegracion Integracion { get; set; } = null!;
    public IntgConector Conector { get; set; } = null!;

    public ICollection<IntgEmpresaIntegracionConector> EmpresaIntegracionConectores { get; set; }
        = new List<IntgEmpresaIntegracionConector>();
}
