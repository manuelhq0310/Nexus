using Nexus.Application.DTOs.Payana;

namespace Nexus.Application.Interfaces.Services
{
    public interface IPayanaIntegracionService
    {
		Task<EjecutarConsultaResponse> EjecutarConsultaAsync(int idCompania, string descripcion, string? paginacionStr, 
															 string? parametrosStr, string connikey, string conniToken);
	}
}
