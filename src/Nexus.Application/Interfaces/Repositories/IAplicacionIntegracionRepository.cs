using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Interfaces.Repositories;

public interface IAplicacionIntegracionRepository
{
    Task<IEnumerable<IntgAplicacionIntegracion>> GetAllAsync(bool soloActivas, CancellationToken cancellationToken = default);
    Task<IEnumerable<IntgAplicacionIntegracion>> GetByAplicacionAsync(long aplicacionId, bool soloActivas, CancellationToken cancellationToken = default);
    Task<IEnumerable<IntgAplicacionIntegracion>> GetByIntegracionAsync(long integracionId, bool soloActivas, CancellationToken cancellationToken = default);
    Task<IntgAplicacionIntegracion?> GetByCompositeKeyAsync(long aplicacionId, long integracionId, CancellationToken cancellationToken = default);
    Task AddAsync(IntgAplicacionIntegracion entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
