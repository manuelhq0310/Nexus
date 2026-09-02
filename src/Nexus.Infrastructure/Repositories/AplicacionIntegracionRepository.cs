using Microsoft.EntityFrameworkCore;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repositories;

public class AplicacionIntegracionRepository : IAplicacionIntegracionRepository
{
    private readonly NexusDbContext _context;
    public AplicacionIntegracionRepository(NexusDbContext context) => _context = context;

    private IQueryable<IntgAplicacionIntegracion> ConDetalle() =>
        _context.IntgAplicacionIntegraciones
            .Include(ai => ai.Aplicacion)
            .Include(ai => ai.Integracion);

    public async Task<IEnumerable<IntgAplicacionIntegracion>> GetAllAsync(bool soloActivas, CancellationToken cancellationToken = default)
    {
        var query = ConDetalle();
        if (soloActivas) query = query.Where(ai => ai.Estado);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<IntgAplicacionIntegracion>> GetByAplicacionAsync(long aplicacionId, bool soloActivas, CancellationToken cancellationToken = default)
    {
        var query = ConDetalle().Where(ai => ai.AplicacionId == aplicacionId);
        if (soloActivas) query = query.Where(ai => ai.Estado);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<IntgAplicacionIntegracion>> GetByIntegracionAsync(long integracionId, bool soloActivas, CancellationToken cancellationToken = default)
    {
        var query = ConDetalle().Where(ai => ai.IntegracionId == integracionId);
        if (soloActivas) query = query.Where(ai => ai.Estado);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IntgAplicacionIntegracion?> GetByCompositeKeyAsync(long aplicacionId, long integracionId, CancellationToken cancellationToken = default) =>
        await ConDetalle().FirstOrDefaultAsync(ai => ai.AplicacionId == aplicacionId && ai.IntegracionId == integracionId, cancellationToken);

    public async Task AddAsync(IntgAplicacionIntegracion entity, CancellationToken cancellationToken = default) =>
        await _context.IntgAplicacionIntegraciones.AddAsync(entity, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
