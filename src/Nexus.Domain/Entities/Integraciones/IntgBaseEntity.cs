namespace Nexus.Domain.Entities.Integraciones;

/// <summary>
/// Clase base para las entidades del módulo de Integraciones. A diferencia de
/// <see cref="BaseEntity"/> (que usa Guid), estas tablas usan Id numérico
/// autoincremental (BIGINT) según lo especificado para este módulo.
/// </summary>
public abstract class IntgBaseEntity
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    /// <summary>1: Activo, 0: Inactivo.</summary>
    public bool Estado { get; set; } = true;
}
