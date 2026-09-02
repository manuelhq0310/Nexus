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

    // Navegación
    public ICollection<IntgAplicacionIntegracion> AplicacionIntegraciones { get; set; } = new List<IntgAplicacionIntegracion>();
    public ICollection<IntgAplicacionEmpresa> AplicacionEmpresas { get; set; } = new List<IntgAplicacionEmpresa>();
    public ICollection<IntgAplicacionConector> AplicacionConectores { get; set; } = new List<IntgAplicacionConector>();
}
