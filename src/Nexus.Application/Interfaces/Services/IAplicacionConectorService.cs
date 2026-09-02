using Nexus.Application.DTOs.AplicacionConectores;

namespace Nexus.Application.Interfaces.Services;

public interface IAplicacionConectorService
{
    Task<IEnumerable<AplicacionConectorDto>> ObtenerPorAplicacionAsync(long aplicacionId);
    Task<AplicacionConectorDto?> ObtenerPorIdAsync(long id);
    Task<AplicacionConectorDto> CrearAsync(CrearAplicacionConectorDto dto);
    Task<bool> ActualizarAsync(long id, ActualizarAplicacionConectorDto dto);
    Task<bool> CambiarEstadoAsync(long id, bool activo);
}
