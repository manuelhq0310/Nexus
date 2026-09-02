using Nexus.Application.DTOs.EmpresaConectores;

namespace Nexus.Application.Interfaces.Services;

/// <summary>Gestiona la relación directa Empresa &lt;-&gt; Conector (relación 1 a 1).</summary>
public interface IEmpresaConectorService
{
    Task<IEnumerable<EmpresaConectorDto>> ObtenerTodasAsync(bool soloActivas = true);
    Task<EmpresaConectorDto?> ObtenerPorEmpresaAsync(long empresaId);
    Task<EmpresaConectorDto?> ObtenerPorIdAsync(long id);
    Task<EmpresaConectorDto> CrearAsync(AsignarEmpresaConectorDto dto);
    Task<bool> CambiarConectorAsync(long id, long nuevoConectorId);
    Task<bool> CambiarEstadoAsync(long id, bool activo);
}
