using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Interfaces.Repositories;

public interface IEmpresaConectorRepository
{
    Task<IEnumerable<IntgEmpresaConector>> GetAllAsync(bool soloActivas, CancellationToken cancellationToken = default);
    Task<IntgEmpresaConector?> GetByEmpresaAsync(long empresaId, CancellationToken cancellationToken = default);
    Task<IntgEmpresaConector?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task AddAsync(IntgEmpresaConector entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
