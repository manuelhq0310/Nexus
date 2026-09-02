using Microsoft.EntityFrameworkCore;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repositories;

public class ConectorRepository : IConectorRepository
{
    private readonly NexusDbContext _context;
    public ConectorRepository(NexusDbContext context) => _context = context;

    public async Task<IEnumerable<IntgConector>> GetAllAsync(bool soloActivos, CancellationToken cancellationToken = default)
    {
        var query = _context.IntgConectores.AsQueryable();
        if (soloActivos) query = query.Where(c => c.Estado);
        return await query.OrderBy(c => c.Nombre).ToListAsync(cancellationToken);
    }

    public async Task<IntgConector?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await _context.IntgConectores.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<bool> ExistsByNombreAsync(string nombre, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.IntgConectores.Where(c => c.Nombre == nombre);
        if (excludeId.HasValue) query = query.Where(c => c.Id != excludeId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(IntgConector conector, CancellationToken cancellationToken = default) =>
        await _context.IntgConectores.AddAsync(conector, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
