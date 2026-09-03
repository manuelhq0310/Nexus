namespace Nexus.Presentation.Models;

/// <summary>
/// Configuración de una consulta genérica al ERP a través del Conector UnoE.
/// La API usa el mismo esquema tanto para leer como para crear/actualizar (POST y PUT
/// reciben este mismo objeto completo), por lo que aquí se modela con una sola clase
/// en vez de separar en Crear/Actualizar como en los demás módulos.
/// </summary>
public class UnoEConsultaConfigDto
{
    public long Id { get; set; }

    /// <summary>Identificador único de negocio de la consulta. Ej: "CONSULTAR_SALDO_PROVEEDOR".</summary>
    public string CodigoConsulta { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    /// <summary>Nombre de la conexión configurada del lado de UnoE que se debe usar.</summary>
    public string? NombreConexion { get; set; }

    /// <summary>Identificador del proveedor/reporte dentro de UnoE que resuelve esta consulta.</summary>
    public string? IdProveedor { get; set; }

    /// <summary>Plantilla XML con los parámetros que espera la consulta en UnoE.</summary>
    public string? PlantillaParametrosXml { get; set; }

    public bool Estado { get; set; } = true;

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
