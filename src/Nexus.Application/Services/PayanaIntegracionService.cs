using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Nexus.Application.DTOs.Auth;
using Nexus.Application.DTOs.IntegracionRouter;
using Nexus.Application.DTOs.Payana;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Application.Services
{
    public class PayanaIntegracionService : IPayanaIntegracionService
	{
		private readonly IAuthService _authService;
		private readonly IIntegracionRouterService _routerService;
		private readonly ILogger<IntegracionRouterService> _logger;
		private const string PAYANA = "PAYANA";

		public PayanaIntegracionService(
			IAuthService authService,
			IIntegracionRouterService routerService,
			ILogger<IntegracionRouterService> logger)
		{
			_authService = authService;
			_routerService = routerService;
			_logger = logger;
		}

		public async Task<EjecutarConsultaResponse> EjecutarConsultaAsync(int idCompania, string descripcion, string? paginacionStr, 
																		  string? parametrosStr, string connikey, string conniToken)
		{
			_logger.LogInformation("Iniciando ejecución de consulta para Compañía: {IdCompania}, Descripción/Consulta: {Descripcion}", idCompania, descripcion);

			AutenticarAplicacionRequestDto autenticacionRequest = new() { CodigoAplicacion = PAYANA, ClientId = connikey, ClientSecret = conniToken };
			await _authService.AutenticarAplicacionAsync(autenticacionRequest);

			var parametros = ParsearParametros(parametrosStr);
			var request = new EjecutarIntegracionRequest(PAYANA, descripcion, idCompania, parametros);
			var integracionResponse = await _routerService.ProcesarIntegracionAsync(request);
			var response = new EjecutarConsultaResponse(integracionResponse.Exitoso ? 0 : 1, integracionResponse.Mensaje, integracionResponse.Data);

			_logger.LogInformation("Finalizada ejecución de consulta para Compañía: {IdCompania}, Descripción: {Descripcion}. Éxito: {Exitoso}", idCompania, descripcion, integracionResponse.Exitoso);

			return response;
		}

		private dynamic ParsearParametros(string? parametrosStr)
		{
			var rootObject = new JObject();
			var parametrosObject = new JObject();

			if (!string.IsNullOrWhiteSpace(parametrosStr))
			{
				var pares = parametrosStr.Split('|');
				foreach (var par in pares)
				{
					var partes = par.Split('=');
					if (partes.Length == 2)
					{
						var clave = partes[0].Trim();
						var valor = partes[1].Trim();

						parametrosObject[clave] = valor;
					}
				}
			}

			rootObject["parametros"] = parametrosObject;
			return rootObject;
		}
	}
}
