using Nexus.Application.DTOs.Empresas;

namespace Nexus.Application.Interfaces.Services;

/// <summary>
/// Administra las compañías del grupo empresarial.
/// </summary>
public interface IEmpresaService
{
    Task<IEnumerable<EmpresaDto>> ObtenerTodasAsync(bool soloActivas = true);
    Task<EmpresaDto?> ObtenerPorIdAsync(long id);
    Task<EmpresaDto?> ObtenerPorIdentificacionAsync(string tipoIdentificacion, string numeroIdentificacion);
    Task<EmpresaDto> CrearAsync(CrearEmpresaDto dto);
    Task<bool> ActualizarAsync(long id, ActualizarEmpresaDto dto);
    Task<bool> CambiarEstadoAsync(long id, bool activo);
}
