using Microsoft.EntityFrameworkCore;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repositories;

public class EmpresaConectorRepository : IEmpresaConectorRepository
{
    private readonly NexusDbContext _context;
    public EmpresaConectorRepository(NexusDbContext context) => _context = context;

    private IQueryable<IntgEmpresaConector> ConDetalle() =>
        _context.IntgEmpresaConectores
            .Include(ec => ec.Empresa)
            .Include(ec => ec.Conector);

    public async Task<IEnumerable<IntgEmpresaConector>> GetAllAsync(bool soloActivas, CancellationToken cancellationToken = default)
    {
        var query = ConDetalle();
        if (soloActivas) query = query.Where(ec => ec.Estado);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IntgEmpresaConector?> GetByEmpresaAsync(long empresaId, CancellationToken cancellationToken = default) =>
        await ConDetalle().FirstOrDefaultAsync(ec => ec.EmpresaId == empresaId, cancellationToken);

    public async Task<IntgEmpresaConector?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await ConDetalle().FirstOrDefaultAsync(ec => ec.Id == id, cancellationToken);

    public async Task AddAsync(IntgEmpresaConector entity, CancellationToken cancellationToken = default) =>
        await _context.IntgEmpresaConectores.AddAsync(entity, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
