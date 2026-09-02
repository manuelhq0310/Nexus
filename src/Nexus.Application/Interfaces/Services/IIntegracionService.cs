using Nexus.Application.DTOs.Integraciones;

namespace Nexus.Application.Interfaces.Services;

public interface IIntegracionService
{
    Task<IEnumerable<IntegracionDto>> ObtenerTodasAsync(bool soloActivas = true);
    Task<IntegracionDto?> ObtenerPorIdAsync(long id);
    Task<IntegracionDto?> ObtenerPorCodigoAccionAsync(string codigoAccion);
    Task<IntegracionDto> CrearAsync(CrearIntegracionDto dto);
    Task<bool> ActualizarAsync(long id, ActualizarIntegracionDto dto);
    Task<bool> CambiarEstadoAsync(long id, bool activo);
}
