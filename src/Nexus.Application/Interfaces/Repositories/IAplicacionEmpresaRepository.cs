using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Interfaces.Repositories;

public interface IAplicacionEmpresaRepository
{
    Task<IEnumerable<IntgAplicacionEmpresa>> GetAllAsync(bool soloActivas, CancellationToken cancellationToken = default);
    Task<IEnumerable<IntgAplicacionEmpresa>> GetByAplicacionAsync(long aplicacionId, bool soloActivas, CancellationToken cancellationToken = default);
    Task<IEnumerable<IntgAplicacionEmpresa>> GetByEmpresaAsync(long empresaId, bool soloActivas, CancellationToken cancellationToken = default);
    Task<IntgAplicacionEmpresa?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long aplicacionId, long empresaId, CancellationToken cancellationToken = default);
    Task AddAsync(IntgAplicacionEmpresa entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
