using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Interfaces.Repositories;

public interface IIntegracionRepository
{
    Task<IEnumerable<IntgIntegracion>> GetAllAsync(bool soloActivas, CancellationToken cancellationToken = default);
    Task<IntgIntegracion?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IntgIntegracion?> GetByCodigoAccionAsync(string codigoAccion, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodigoAccionAsync(string codigoAccion, long? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(IntgIntegracion integracion, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
