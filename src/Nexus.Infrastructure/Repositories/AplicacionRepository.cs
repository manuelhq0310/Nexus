using Microsoft.EntityFrameworkCore;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repositories;

public class AplicacionRepository : IAplicacionRepository
{
    private readonly NexusDbContext _context;
    public AplicacionRepository(NexusDbContext context) => _context = context;

    public async Task<IEnumerable<IntgAplicacion>> GetAllAsync(bool soloActivas, CancellationToken cancellationToken = default)
    {
        var query = _context.IntgAplicaciones.AsQueryable();
        if (soloActivas) query = query.Where(a => a.Estado);
        return await query.OrderBy(a => a.Nombre).ToListAsync(cancellationToken);
    }

    public async Task<IntgAplicacion?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await _context.IntgAplicaciones.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IntgAplicacion?> GetByCodigoAsync(string codigoApp, CancellationToken cancellationToken = default) =>
        await _context.IntgAplicaciones.FirstOrDefaultAsync(a => a.CodigoApp == codigoApp, cancellationToken);

    public async Task<bool> ExistsByCodigoAsync(string codigoApp, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.IntgAplicaciones.Where(a => a.CodigoApp == codigoApp);
        if (excludeId.HasValue) query = query.Where(a => a.Id != excludeId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(IntgAplicacion aplicacion, CancellationToken cancellationToken = default) =>
        await _context.IntgAplicaciones.AddAsync(aplicacion, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
