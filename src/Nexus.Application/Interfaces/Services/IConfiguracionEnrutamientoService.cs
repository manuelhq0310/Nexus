using Nexus.Application.DTOs.ConfiguracionEnrutamiento;

namespace Nexus.Application.Interfaces.Services;

/// <summary>Gestiona la matriz de enrutamiento Empresa + Integración-Conector (endpoint "ConfiguracionEnrutamiento").</summary>
public interface IConfiguracionEnrutamientoService
{
    Task<EmpresaIntegracionConectorDto?> ObtenerPorIdAsync(long id);
    Task<IEnumerable<EmpresaIntegracionConectorDto>> ObtenerPorEmpresaAsync(long empresaId);
    Task<EmpresaIntegracionConectorDto> CrearAsync(CrearEmpresaIntegracionConectorDto dto);
    Task<bool> ActualizarAsync(long id, ActualizarEmpresaIntegracionConectorDto dto);

    /// <summary>Resuelve en caliente la URL/cola final para una empresa + código de acción.</summary>
    Task<RutaEnrutamientoResueltaDto?> ResolverAsync(long empresaId, string codigoAccion);
}
