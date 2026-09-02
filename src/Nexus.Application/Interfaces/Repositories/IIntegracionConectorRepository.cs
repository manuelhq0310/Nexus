using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Interfaces.Repositories;

public interface IIntegracionConectorRepository
{
    Task<IntgIntegracionConector?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<IntgIntegracionConector>> GetByIntegracionAsync(long integracionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<IntgIntegracionConector>> GetByConectorAsync(long conectorId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long integracionId, long conectorId, CancellationToken cancellationToken = default);
    Task AddAsync(IntgIntegracionConector entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
