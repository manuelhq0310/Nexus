namespace Nexus.Domain.Entities.Integraciones;

/// <summary>Relación Aplicación &lt;-&gt; Integración: qué acciones de negocio puede ejecutar la aplicación.</summary>
public class IntgAplicacionIntegracion : IntgSimpleEntity
{
    public long AplicacionId { get; set; }
    public long IntegracionId { get; set; }

    // Navegación
    public IntgAplicacion Aplicacion { get; set; } = null!;
    public IntgIntegracion Integracion { get; set; } = null!;
}
