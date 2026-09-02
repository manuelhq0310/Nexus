namespace Nexus.Domain.Entities.Integraciones;

/// <summary>
/// Microservicio/API intermediaria conectada a cada ERP o sistema destino,
/// centralizando sus parámetros base de red.
/// </summary>
public class IntgConector : IntgBaseEntity
{
    /// <summary>Ej: "Conector UnoE", "Conector SAP ECC", "Conector SAP S/4HANA".</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Ej: "REST", "SOAP", "RFC".</summary>
    public string TipoProtocolo { get; set; } = string.Empty;

    /// <summary>Ej: "https://api.conector-sapecc.internal".</summary>
    public string UrlBase { get; set; } = string.Empty;

    // Navegación
    public ICollection<IntgIntegracionConector> IntegracionConectores { get; set; }
        = new List<IntgIntegracionConector>();
    public ICollection<IntgEmpresaConector> EmpresaConectores { get; set; } = new List<IntgEmpresaConector>();
    public ICollection<IntgAplicacionConector> AplicacionConectores { get; set; } = new List<IntgAplicacionConector>();
}
