using Nexus.Application.DTOs.Aplicaciones;

namespace Nexus.Application.Interfaces.Services;

public interface IAplicacionService
{
    Task<IEnumerable<AplicacionDto>> ObtenerTodasAsync(bool soloActivas = true);
    Task<AplicacionDto?> ObtenerPorIdAsync(long id);
    Task<AplicacionDto?> ObtenerPorCodigoAsync(string codigoApp);
    Task<AplicacionDto> CrearAsync(CrearAplicacionDto dto);
    Task<bool> ActualizarAsync(long id, ActualizarAplicacionDto dto);
    Task<bool> CambiarEstadoAsync(long id, bool activo);
}
