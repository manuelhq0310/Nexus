using Nexus.Application.DTOs.IntegracionConectores;

namespace Nexus.Application.Interfaces.Services;

public interface IIntegracionConectorService
{
    Task<IntegracionConectorDto?> ObtenerPorIdAsync(long id);
    Task<IEnumerable<IntegracionConectorDto>> ObtenerPorIntegracionAsync(long integracionId);
    Task<IEnumerable<IntegracionConectorDto>> ObtenerPorConectorAsync(long conectorId);
    Task<IntegracionConectorDto> CrearAsync(CrearIntegracionConectorDto dto);
    Task<bool> ActualizarAsync(long id, ActualizarIntegracionConectorDto dto);
    Task<bool> CambiarEstadoAsync(long id, bool activo);
}
