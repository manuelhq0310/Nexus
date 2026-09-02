using Microsoft.EntityFrameworkCore;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repositories;

public class IntegracionConectorRepository : IIntegracionConectorRepository
{
    private readonly NexusDbContext _context;
    public IntegracionConectorRepository(NexusDbContext context) => _context = context;

    private IQueryable<IntgIntegracionConector> ConDetalle() =>
        _context.IntgIntegracionConectores
            .Include(ic => ic.Integracion)
            .Include(ic => ic.Conector);

    public async Task<IntgIntegracionConector?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await ConDetalle().FirstOrDefaultAsync(ic => ic.Id == id, cancellationToken);

    public async Task<IEnumerable<IntgIntegracionConector>> GetByIntegracionAsync(long integracionId, CancellationToken cancellationToken = default) =>
        await ConDetalle().Where(ic => ic.IntegracionId == integracionId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<IntgIntegracionConector>> GetByConectorAsync(long conectorId, CancellationToken cancellationToken = default) =>
        await ConDetalle().Where(ic => ic.ConectorId == conectorId).ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(long integracionId, long conectorId, CancellationToken cancellationToken = default) =>
        await _context.IntgIntegracionConectores.AnyAsync(
            ic => ic.IntegracionId == integracionId && ic.ConectorId == conectorId, cancellationToken);

    public async Task AddAsync(IntgIntegracionConector entity, CancellationToken cancellationToken = default) =>
        await _context.IntgIntegracionConectores.AddAsync(entity, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
