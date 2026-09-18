using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Interfaces.Repositories;

public interface IEmpresaRepository
{
    Task<IEnumerable<IntgEmpresa>> GetAllAsync(bool soloActivas, CancellationToken cancellationToken = default);
    Task<IntgEmpresa?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodigoAsync(int codigoEmpresa, long? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(IntgEmpresa empresa, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
