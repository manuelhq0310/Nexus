using Nexus.Application.DTOs.IntegracionRouter;

namespace Nexus.Application.Interfaces.Services
{
    public interface IIntegracionRouterService
    {
        Task<EjecutarIntegracionResponse> ProcesarIntegracionAsync(EjecutarIntegracionRequest request);
    }
}
