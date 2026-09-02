using Microsoft.EntityFrameworkCore;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repositories;

public class AplicacionEmpresaRepository : IAplicacionEmpresaRepository
{
    private readonly NexusDbContext _context;
    public AplicacionEmpresaRepository(NexusDbContext context) => _context = context;

    private IQueryable<IntgAplicacionEmpresa> ConDetalle() =>
        _context.IntgAplicacionEmpresas
            .Include(ae => ae.Aplicacion)
            .Include(ae => ae.Empresa);

    public async Task<IEnumerable<IntgAplicacionEmpresa>> GetAllAsync(bool soloActivas, CancellationToken cancellationToken = default)
    {
        var query = ConDetalle();
        if (soloActivas) query = query.Where(ae => ae.Estado);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<IntgAplicacionEmpresa>> GetByAplicacionAsync(long aplicacionId, bool soloActivas, CancellationToken cancellationToken = default)
    {
        var query = ConDetalle().Where(ae => ae.AplicacionId == aplicacionId);
        if (soloActivas) query = query.Where(ae => ae.Estado);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<IntgAplicacionEmpresa>> GetByEmpresaAsync(long empresaId, bool soloActivas, CancellationToken cancellationToken = default)
    {
        var query = ConDetalle().Where(ae => ae.EmpresaId == empresaId);
        if (soloActivas) query = query.Where(ae => ae.Estado);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IntgAplicacionEmpresa?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await ConDetalle().FirstOrDefaultAsync(ae => ae.Id == id, cancellationToken);

    public async Task<bool> ExistsAsync(long aplicacionId, long empresaId, CancellationToken cancellationToken = default) =>
        await _context.IntgAplicacionEmpresas.AnyAsync(ae => ae.AplicacionId == aplicacionId && ae.EmpresaId == empresaId, cancellationToken);

    public async Task AddAsync(IntgAplicacionEmpresa entity, CancellationToken cancellationToken = default) =>
        await _context.IntgAplicacionEmpresas.AddAsync(entity, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
