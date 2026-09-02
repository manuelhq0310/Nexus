namespace Nexus.Domain.Entities.Integraciones;

/// <summary>
/// Relación directa Empresa &lt;-&gt; Conector: qué conector utiliza cada empresa.
/// Es una relación 1 a 1 (una empresa tiene a lo sumo un conector asociado a la vez).
/// </summary>
public class IntgEmpresaConector : IntgSimpleEntity
{
    public long EmpresaId { get; set; }
    public long ConectorId { get; set; }

    // Navegación
    public IntgEmpresa Empresa { get; set; } = null!;
    public IntgConector Conector { get; set; } = null!;
}
