using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Interfaces.Repositories;

public interface IAplicacionRepository
{
    Task<IEnumerable<IntgAplicacion>> GetAllAsync(bool soloActivas, CancellationToken cancellationToken = default);
    Task<IntgAplicacion?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IntgAplicacion?> GetByCodigoAsync(string codigoApp, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodigoAsync(string codigoApp, long? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(IntgAplicacion aplicacion, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
