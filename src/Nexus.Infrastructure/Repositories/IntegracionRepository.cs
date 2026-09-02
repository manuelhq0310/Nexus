using Microsoft.EntityFrameworkCore;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repositories;

public class IntegracionRepository : IIntegracionRepository
{
    private readonly NexusDbContext _context;
    public IntegracionRepository(NexusDbContext context) => _context = context;

    public async Task<IEnumerable<IntgIntegracion>> GetAllAsync(bool soloActivas, CancellationToken cancellationToken = default)
    {
        var query = _context.IntgIntegraciones.AsQueryable();
        if (soloActivas) query = query.Where(i => i.Estado);
        return await query.OrderBy(i => i.Nombre).ToListAsync(cancellationToken);
    }

    public async Task<IntgIntegracion?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await _context.IntgIntegraciones.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<IntgIntegracion?> GetByCodigoAccionAsync(string codigoAccion, CancellationToken cancellationToken = default) =>
        await _context.IntgIntegraciones.FirstOrDefaultAsync(i => i.CodigoAccion == codigoAccion, cancellationToken);

    public async Task<bool> ExistsByCodigoAccionAsync(string codigoAccion, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.IntgIntegraciones.Where(i => i.CodigoAccion == codigoAccion);
        if (excludeId.HasValue) query = query.Where(i => i.Id != excludeId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(IntgIntegracion integracion, CancellationToken cancellationToken = default) =>
        await _context.IntgIntegraciones.AddAsync(integracion, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
