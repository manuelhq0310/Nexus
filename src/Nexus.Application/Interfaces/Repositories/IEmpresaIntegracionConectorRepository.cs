using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Interfaces.Repositories;

/// <summary>Repositorio para la matriz de enrutamiento (endpoint "ConfiguracionEnrutamiento").</summary>
public interface IEmpresaIntegracionConectorRepository
{
    Task<IntgEmpresaIntegracionConector?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<IntgEmpresaIntegracionConector>> GetByEmpresaAsync(long empresaId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long empresaId, long integracionConectorId, CancellationToken cancellationToken = default);

    /// <summary>Búsqueda usada por el endpoint "resolver": empresa + código de acción -> combinación activa.</summary>
    Task<IntgEmpresaIntegracionConector?> GetActivaPorEmpresaYCodigoAccionAsync(long empresaId, string codigoAccion, CancellationToken cancellationToken = default);

    Task AddAsync(IntgEmpresaIntegracionConector entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
