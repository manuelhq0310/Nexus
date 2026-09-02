namespace Nexus.Domain.Entities.Integraciones;

/// <summary>
/// Representa a las diferentes organizaciones/compañías pertenecientes al grupo empresarial.
/// </summary>
public class IntgEmpresa : IntgBaseEntity
{
    /// <summary>Ej: "NIT", "RUC", "CEDULA".</summary>
    public string TipoIdentificacion { get; set; } = string.Empty;

    /// <summary>Ej: "900.123.456-1".</summary>
    public string NumeroIdentificacion { get; set; } = string.Empty;

    public string NombreRazonSocial { get; set; } = string.Empty;

    // Navegación
    public ICollection<IntgEmpresaIntegracionConector> EmpresaIntegracionConectores { get; set; }
        = new List<IntgEmpresaIntegracionConector>();
    public ICollection<IntgEmpresaConector> EmpresaConectores { get; set; } = new List<IntgEmpresaConector>();
    public ICollection<IntgAplicacionEmpresa> AplicacionEmpresas { get; set; } = new List<IntgAplicacionEmpresa>();
}
