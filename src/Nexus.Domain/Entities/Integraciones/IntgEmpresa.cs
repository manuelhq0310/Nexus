namespace Nexus.Domain.Entities.Integraciones;

/// <summary>
/// Representa a las diferentes organizaciones/compañías pertenecientes al grupo empresarial.
/// </summary>
public class IntgEmpresa : IntgBaseEntity
{
    public int CodigoEmpresa { get; set; }

    public string NombreRazonSocial { get; set; } = string.Empty;

    // Navegación
    public ICollection<IntgEmpresaIntegracionConector> EmpresaIntegracionConectores { get; set; }
        = new List<IntgEmpresaIntegracionConector>();
    public IntgEmpresaConector? EmpresaConector { get; set; }
    public ICollection<IntgAplicacionEmpresa> AplicacionEmpresas { get; set; } = new List<IntgAplicacionEmpresa>();
}
