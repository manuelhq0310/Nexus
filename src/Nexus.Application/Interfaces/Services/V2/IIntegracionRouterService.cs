using Nexus.Application.DTOs.IntegracionRouter;

namespace Nexus.Application.Interfaces.Services.V2
{
    public interface IIntegracionRouterService
    {
		Task<EjecutarConsultaResponse> EjecutarConsultaAsync(int idCompania, string descripcion, string codigoAplicacion, string? paginacionStr, 
															 string? parametrosStr, string connikey, string conniToken);

		Task<EjecutarConsultaResponse> EjecutarIntegracionAsync(int idCompania, string? idDocumento, string? nombreDocumento,
															  string codigoAplicacion, dynamic payload, string connikey, string conniToken);
	}
}
