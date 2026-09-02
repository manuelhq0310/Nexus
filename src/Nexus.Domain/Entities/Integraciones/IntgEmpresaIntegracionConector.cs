namespace Nexus.Domain.Entities.Integraciones;

/// <summary>
/// Matriz de enrutamiento (endpoint "ConfiguracionEnrutamiento" en la API): asigna la
/// combinación específica de Integración-Conector a cada Empresa, con las credenciales
/// de autenticación propias de esa empresa para ese enrutamiento.
/// </summary>
public class IntgEmpresaIntegracionConector : IntgBaseEntity
{
    public long EmpresaId { get; set; }
    public long IntegracionConectorId { get; set; }

    public bool RequiereAutenticacion { get; set; }
    public string? ApiKey { get; set; }
    public string? TokenBearer { get; set; }

    // Navegación
    public IntgEmpresa Empresa { get; set; } = null!;
    public IntgIntegracionConector IntegracionConector { get; set; } = null!;
}
