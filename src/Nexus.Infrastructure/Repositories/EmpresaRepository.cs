using Microsoft.EntityFrameworkCore;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repositories;

public class EmpresaRepository : IEmpresaRepository
{
    private readonly NexusDbContext _context;
    public EmpresaRepository(NexusDbContext context) => _context = context;

    public async Task<IEnumerable<IntgEmpresa>> GetAllAsync(bool soloActivas, CancellationToken cancellationToken = default)
    {
        var query = _context.IntgEmpresas.AsQueryable();
        if (soloActivas) query = query.Where(e => e.Estado);
        return await query.OrderBy(e => e.NombreRazonSocial).ToListAsync(cancellationToken);
    }

    public async Task<IntgEmpresa?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await _context.IntgEmpresas.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IntgEmpresa?> GetByIdentificacionAsync(string tipoIdentificacion, string numeroIdentificacion, CancellationToken cancellationToken = default) =>
        await _context.IntgEmpresas.FirstOrDefaultAsync(
            e => e.TipoIdentificacion == tipoIdentificacion && e.NumeroIdentificacion == numeroIdentificacion,
            cancellationToken);

    public async Task<bool> ExistsByIdentificacionAsync(string tipoIdentificacion, string numeroIdentificacion, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.IntgEmpresas.Where(
            e => e.TipoIdentificacion == tipoIdentificacion && e.NumeroIdentificacion == numeroIdentificacion);
        if (excludeId.HasValue) query = query.Where(e => e.Id != excludeId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(IntgEmpresa empresa, CancellationToken cancellationToken = default) =>
        await _context.IntgEmpresas.AddAsync(empresa, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
