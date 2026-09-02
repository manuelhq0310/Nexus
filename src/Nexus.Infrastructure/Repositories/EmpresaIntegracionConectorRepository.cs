using Microsoft.EntityFrameworkCore;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repositories;

public class EmpresaIntegracionConectorRepository : IEmpresaIntegracionConectorRepository
{
    private readonly NexusDbContext _context;
    public EmpresaIntegracionConectorRepository(NexusDbContext context) => _context = context;

    private IQueryable<IntgEmpresaIntegracionConector> ConDetalle() =>
        _context.IntgEmpresaIntegracionConectores
            .Include(eic => eic.Empresa)
            .Include(eic => eic.IntegracionConector).ThenInclude(ic => ic.Integracion)
            .Include(eic => eic.IntegracionConector).ThenInclude(ic => ic.Conector);

    public async Task<IntgEmpresaIntegracionConector?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await ConDetalle().FirstOrDefaultAsync(eic => eic.Id == id, cancellationToken);

    public async Task<IEnumerable<IntgEmpresaIntegracionConector>> GetByEmpresaAsync(long empresaId, CancellationToken cancellationToken = default) =>
        await ConDetalle().Where(eic => eic.EmpresaId == empresaId).ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(long empresaId, long integracionConectorId, CancellationToken cancellationToken = default) =>
        await _context.IntgEmpresaIntegracionConectores.AnyAsync(
            eic => eic.EmpresaId == empresaId && eic.IntegracionConectorId == integracionConectorId, cancellationToken);

    public async Task<IntgEmpresaIntegracionConector?> GetActivaPorEmpresaYCodigoAccionAsync(long empresaId, string codigoAccion, CancellationToken cancellationToken = default) =>
        await ConDetalle().FirstOrDefaultAsync(
            eic => eic.EmpresaId == empresaId
                && eic.Estado
                && eic.IntegracionConector.Estado
                && eic.IntegracionConector.Integracion.CodigoAccion == codigoAccion,
            cancellationToken);

    public async Task AddAsync(IntgEmpresaIntegracionConector entity, CancellationToken cancellationToken = default) =>
        await _context.IntgEmpresaIntegracionConectores.AddAsync(entity, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
