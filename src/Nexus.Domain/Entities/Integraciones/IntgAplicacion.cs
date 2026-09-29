namespace Nexus.Domain.Entities.Integraciones;

/// <summary>
/// Aplicación del grupo empresarial que requiere integraciones. Es la entidad central que
/// asocia Integraciones, Empresas y Conectores (con credenciales propias por conector).
/// </summary>
public class IntgAplicacion : IntgSimpleEntity
{
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Identificador único de negocio, ej: "PORTAL_VIAJES".</summary>
    public string CodigoApp { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    // --- Credenciales M2M (Opcionales, se llenan solo si se habilita la integración M2M) ---
    /// <summary>Identificador público de cliente para autenticación OAuth/M2M, ej: "app_payana_prod_8f9a".</summary>
    public string? ClientId { get; set; }

    /// <summary>Hash SHA256 del ClientSecret activo.</summary>
    public string? ClientSecretHash { get; set; }

    public DateTime? UltimaRotacionSecreto { get; set; }

    // --- Onboarding Efímero (Opcional, activo solo durante la fase de reclamo) ---
    /// <summary>Token único y temporal para que la aplicación reclame su ClientSecret por POST.</summary>
    public string? OnboardingToken { get; set; }

    /// <summary>Fecha límite para que el cliente reclame sus credenciales (ej: 24 horas).</summary>
    public DateTime? FechaExpiracionOnboarding { get; set; }

    /// <summary>Indica si la aplicación ya completó su onboarding M2M.</summary>
    public bool OnboardingCompletado { get; set; } = false;

    // Navegación
    public ICollection<IntgAplicacionIntegracion> AplicacionIntegraciones { get; set; } = new List<IntgAplicacionIntegracion>();
    public ICollection<IntgAplicacionEmpresa> AplicacionEmpresas { get; set; } = new List<IntgAplicacionEmpresa>();
    public ICollection<IntgAplicacionConector> AplicacionConectores { get; set; } = new List<IntgAplicacionConector>();
}
