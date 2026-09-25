using Microsoft.EntityFrameworkCore;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Domain.Enums;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repositories
{
    public class IntgRabbitmqRequestRepository : IIntgRabbitmqRequestRepository
    {
        private readonly NexusDbContext _context;

        public IntgRabbitmqRequestRepository(NexusDbContext context)
        {
            _context = context;
        }

        public async Task<IntgRabbitmqRequest?> GetByIdAsync(string id)
        {
            return await _context.IntgRabbitmqRequests.FirstOrDefaultAsync(r => r.RabbitmqRequestId == id);
        }

        public async Task<IntgRabbitmqRequest> AddAsync(IntgRabbitmqRequest request)
        {
            await _context.IntgRabbitmqRequests.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> UpdateEstadoAsync(string id, EstadoRabbitMqRequest estado, string? respuestaDetalle = null, DateTime? fechaProcesado = null)
        {
            var entity = await _context.IntgRabbitmqRequests.FirstOrDefaultAsync(r => r.RabbitmqRequestId == id);
            if (entity == null) return false;

            entity.Estado = estado;
            entity.RespuestaDetalle = respuestaDetalle ?? entity.RespuestaDetalle;
            entity.FechaProcesado = fechaProcesado ?? DateTime.UtcNow;

            _context.IntgRabbitmqRequests.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<IntgRabbitmqRequest>> GetPendientesAsync()
        {
            return await _context.IntgRabbitmqRequests
                .Where(r => r.Estado == EstadoRabbitMqRequest.Pendiente)
                .OrderBy(r => r.FechaCreacion)
                .ToListAsync();
        }
    }
}
