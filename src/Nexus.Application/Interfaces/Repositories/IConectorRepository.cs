using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Interfaces.Repositories;

public interface IConectorRepository
{
    Task<IEnumerable<IntgConector>> GetAllAsync(bool soloActivos, CancellationToken cancellationToken = default);
    Task<IntgConector?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNombreAsync(string nombre, long? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(IntgConector conector, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
