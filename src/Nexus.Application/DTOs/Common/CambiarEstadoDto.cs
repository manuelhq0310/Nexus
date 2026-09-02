namespace Nexus.Application.DTOs.Common;

/// <summary>
/// Cuerpo estándar para las operaciones de activar/desactivar (soft toggle)
/// usadas por los distintos módulos de catálogo (Empresas, Conectores, Integraciones, etc.).
/// </summary>
public class CambiarEstadoDto
{
    /// <summary>true: Activo, false: Inactivo.</summary>
    public bool Activo { get; set; }
}
