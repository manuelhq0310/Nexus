namespace Nexus.Domain.Entities.Integraciones;

/// <summary>Relación Aplicación &lt;-&gt; Empresa: qué empresas utilizan la aplicación.</summary>
public class IntgAplicacionEmpresa : IntgSimpleEntity
{
    public long AplicacionId { get; set; }
    public long EmpresaId { get; set; }

    // Navegación
    public IntgAplicacion Aplicacion { get; set; } = null!;
    public IntgEmpresa Empresa { get; set; } = null!;
}
