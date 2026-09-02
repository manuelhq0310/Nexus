using Nexus.Application.DTOs.Conectores;

namespace Nexus.Application.Interfaces.Services;

public interface IConectorService
{
    Task<IEnumerable<ConectorDto>> ObtenerTodosAsync(bool soloActivos = true);
    Task<ConectorDto?> ObtenerPorIdAsync(long id);
    Task<ConectorDto> CrearAsync(CrearConectorDto dto);
    Task<bool> ActualizarAsync(long id, ActualizarConectorDto dto);
    Task<bool> CambiarEstadoAsync(long id, bool activo);
}
