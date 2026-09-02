using Microsoft.EntityFrameworkCore;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repositories;

public class AplicacionConectorRepository : IAplicacionConectorRepository
{
    private readonly NexusDbContext _context;
    public AplicacionConectorRepository(NexusDbContext context) => _context = context;

    private IQueryable<IntgAplicacionConector> ConDetalle() =>
        _context.IntgAplicacionConectores
            .Include(ac => ac.Aplicacion)
            .Include(ac => ac.Conector);

    public async Task<IEnumerable<IntgAplicacionConector>> GetByAplicacionAsync(long aplicacionId, CancellationToken cancellationToken = default) =>
        await ConDetalle().Where(ac => ac.AplicacionId == aplicacionId).ToListAsync(cancellationToken);

    public async Task<IntgAplicacionConector?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await ConDetalle().FirstOrDefaultAsync(ac => ac.Id == id, cancellationToken);

    public async Task<bool> ExistsAsync(long aplicacionId, long conectorId, CancellationToken cancellationToken = default) =>
        await _context.IntgAplicacionConectores.AnyAsync(ac => ac.AplicacionId == aplicacionId && ac.ConectorId == conectorId, cancellationToken);

    public async Task AddAsync(IntgAplicacionConector entity, CancellationToken cancellationToken = default) =>
        await _context.IntgAplicacionConectores.AddAsync(entity, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
