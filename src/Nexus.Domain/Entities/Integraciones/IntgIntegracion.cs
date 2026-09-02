namespace Nexus.Domain.Entities.Integraciones;

/// <summary>
/// Catálogo maestro que define las acciones o procesos de negocio estandarizados
/// que la plataforma puede ejecutar.
/// </summary>
public class IntgIntegracion : IntgBaseEntity
{
    /// <summary>Ej: "REGISTRAR_ANTICIPO", "CAUSAR_LEGALIZACION", "SINCRONIZAR_TERCEROS".</summary>
    public string CodigoAccion { get; set; } = string.Empty;

    /// <summary>Ej: "Registro de Anticipos de Viaje".</summary>
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    /// <summary>
    /// Columna presente en el esquema de base de datos (IntgIntegraciones.Tipo) sin uso actual
    /// en el contrato de la API (no aparece en IntegracionDto). Se conserva para mantener el
    /// esquema fiel a la base de datos existente; valor por defecto 0.
    /// </summary>
    public int Tipo { get; set; }

    // Navegación
    public ICollection<IntgIntegracionConector> IntegracionConectores { get; set; }
        = new List<IntgIntegracionConector>();
    public ICollection<IntgAplicacionIntegracion> AplicacionIntegraciones { get; set; } = new List<IntgAplicacionIntegracion>();
}
