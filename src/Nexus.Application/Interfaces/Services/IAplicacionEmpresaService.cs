using Nexus.Application.DTOs.AplicacionEmpresas;

namespace Nexus.Application.Interfaces.Services;

public interface IAplicacionEmpresaService
{
    Task<IEnumerable<AplicacionEmpresaDto>> ObtenerTodasAsync(bool soloActivas = true);
    Task<IEnumerable<AplicacionEmpresaDto>> ObtenerPorAplicacionAsync(long aplicacionId, bool soloActivas = true);
    Task<IEnumerable<AplicacionEmpresaDto>> ObtenerPorEmpresaAsync(long empresaId, bool soloActivas = true);
    Task<AplicacionEmpresaDto?> ObtenerPorIdAsync(long id);
    Task<AplicacionEmpresaDto> CrearAsync(AsignarEmpresaAAplicacionDto dto);
    Task<bool> CambiarEstadoAsync(long id, bool activo);
}
