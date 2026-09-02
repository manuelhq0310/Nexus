using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Interfaces.Repositories;

public interface IAplicacionConectorRepository
{
    Task<IEnumerable<IntgAplicacionConector>> GetByAplicacionAsync(long aplicacionId, CancellationToken cancellationToken = default);
    Task<IntgAplicacionConector?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long aplicacionId, long conectorId, CancellationToken cancellationToken = default);
    Task AddAsync(IntgAplicacionConector entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
