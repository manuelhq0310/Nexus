using Microsoft.EntityFrameworkCore;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repositories
{
    public class UnoEConsultaConfigRepository : IUnoEConsultaConfigRepository
    {
        private readonly NexusDbContext _context;

        public UnoEConsultaConfigRepository(NexusDbContext context)
        {
            _context = context;
        }

        public async Task<UnoEConsultaConfig?> GetByCodigoAsync(string codigoConsulta)
        {
            return await _context.Set<UnoEConsultaConfig>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CodigoConsulta == codigoConsulta && x.Estado);
        }

        public async Task<IEnumerable<UnoEConsultaConfig>> GetAllActivasAsync()
        {
            return await _context.Set<UnoEConsultaConfig>()
                .AsNoTracking()
                .Where(x => x.Estado)
                .ToListAsync();
        }

        public async Task<UnoEConsultaConfig> AddAsync(UnoEConsultaConfig entity)
        {
            await _context.Set<UnoEConsultaConfig>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(UnoEConsultaConfig entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Set<UnoEConsultaConfig>().Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
