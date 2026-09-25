using Nexus.Domain.Entities.Integraciones;
using Nexus.Domain.Enums;

namespace Nexus.Application.Interfaces.Repositories
{
    public interface IIntgRabbitmqRequestRepository
    {
        Task<IntgRabbitmqRequest?> GetByIdAsync(string id);
        Task<IntgRabbitmqRequest> AddAsync(IntgRabbitmqRequest request);
        Task<bool> UpdateEstadoAsync(string id, EstadoRabbitMqRequest estado, string? respuestaDetalle = null, DateTime? fechaProcesado = null);
        Task<IEnumerable<IntgRabbitmqRequest>> GetPendientesAsync();
    }
}
