namespace Nexus.Domain.Entities.Integraciones;

/// <summary>
/// Base para las tablas del módulo de Aplicaciones (IntgAplicacion, IntgAplicacionConector,
/// IntgAplicacionIntegracion, IntgAplicacionEmpresa, IntgEmpresaConector). A diferencia de
/// <see cref="IntgBaseEntity"/>, estas tablas NO tienen columnas de auditoría (CreatedAt/UpdatedAt)
/// en el esquema de base de datos existente.
/// </summary>
public abstract class IntgSimpleEntity
{
    public long Id { get; set; }

    /// <summary>1: Activo, 0: Inactivo.</summary>
    public bool Estado { get; set; } = true;
}
