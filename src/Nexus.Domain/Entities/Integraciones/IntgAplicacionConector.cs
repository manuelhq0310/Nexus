namespace Nexus.Domain.Entities.Integraciones;

/// <summary>
/// Relación Aplicación &lt;-&gt; Conector, con las credenciales que la aplicación usa
/// para autenticarse en ese conector.
/// </summary>
public class IntgAplicacionConector : IntgSimpleEntity
{
    public long AplicacionId { get; set; }
    public long ConectorId { get; set; }

    public string? UsuarioErp { get; set; }
    public string? PasswordErp { get; set; }

    /// <summary>
    /// Campos requeridos por el contrato de la API (CrearAplicacionConectorDto /
    /// ActualizarAplicacionConectorDto) que no estaban en la última foto del esquema de BD
    /// provista; se agregan aquí para que la migración los incluya.
    /// </summary>
    public string? UrlBasePersonalizada { get; set; }
    public string? ApiKey { get; set; }
    public string? TokenBearer { get; set; }

    // Navegación
    public IntgAplicacion Aplicacion { get; set; } = null!;
    public IntgConector Conector { get; set; } = null!;
}
