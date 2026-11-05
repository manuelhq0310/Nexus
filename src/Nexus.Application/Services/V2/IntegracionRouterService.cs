using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Nexus.Application.DTOs.Auth;
using Nexus.Application.DTOs.IntegracionRouter;
using Nexus.Application.Interfaces.Services;
using System.Text.Json;

namespace Nexus.Application.Services.V2
{
    public class IntegracionRouterService : Interfaces.Services.V2.IIntegracionRouterService
	{
		private readonly IAuthService _authService;
		private readonly Interfaces.Services.IIntegracionRouterService _routerService;
		private readonly ILogger<Services.IntegracionRouterService> _logger;

		public IntegracionRouterService(
			IAuthService authService,
            Interfaces.Services.IIntegracionRouterService routerService,
			ILogger<Services.IntegracionRouterService> logger)
		{
			_authService = authService;
			_routerService = routerService;
			_logger = logger;
		}

		public async Task<EjecutarConsultaResponse> EjecutarConsultaAsync(int idCompania, string descripcion, string codigoAplicacion, string? paginacionStr, 
																		  string? parametrosStr, string connikey, string conniToken)
		{
			_logger.LogInformation("Iniciando ejecución de consulta para Compañía: {IdCompania}, Descripción/Consulta: {Descripcion}", idCompania, descripcion);

			AutenticarAplicacionRequestDto autenticacionRequest = new() { CodigoAplicacion = codigoAplicacion, ClientId = connikey, ClientSecret = conniToken };
			await _authService.AutenticarAplicacionAsync(autenticacionRequest);

			var parametros = ParsearParametros(parametrosStr);
			var request = new EjecutarIntegracionRequest(codigoAplicacion, descripcion, idCompania, parametros);
			var integracionResponse = await _routerService.ProcesarIntegracionAsync(request);
			var response = new EjecutarConsultaResponse(integracionResponse.Codigo, integracionResponse.Mensaje, integracionResponse.Data);

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

        public async Task<EjecutarConsultaResponse> EjecutarIntegracionAsync(int idCompania, string? idDocumento, string? nombreDocumento, string codigoAplicacion,
																			  dynamic payload, string connikey, string conniToken)
        {
			_logger.LogInformation("Iniciando ejecución de causación de servicio para Compañía: {IdCompania}, idDocumento: {IdDocumento}", idCompania, idDocumento);

			AutenticarAplicacionRequestDto autenticacionRequest = new() { CodigoAplicacion = codigoAplicacion, ClientId = connikey, ClientSecret = conniToken };
			await _authService.AutenticarAplicacionAsync(autenticacionRequest);

			var queryParams = ConstruirQueryParamsCausacion(idCompania, idDocumento, nombreDocumento);
			EjecutarIntegracionRequest request = new(codigoAplicacion, nombreDocumento, idCompania, payload, queryParams);
			var integracionResponse = await _routerService.ProcesarIntegracionAsync(request);

			EjecutarConsultaResponse? response;
			if (integracionResponse.Data != null)
			{
				string jsonString = JsonSerializer.Serialize(integracionResponse.Data);
				response = JsonSerializer.Deserialize<EjecutarConsultaResponse>(jsonString, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});
				return response!;
			}
			else
			{
				response = new EjecutarConsultaResponse(integracionResponse.Codigo, integracionResponse.Mensaje, integracionResponse.Data);
			}

			return response;
		}

		private Dictionary<string, string> ConstruirQueryParamsCausacion(int idCompania, string? idDocumento, string? nombreDocumento)
		{
			var queryParams = new Dictionary<string, string>
			{
				{ "idCompania", idCompania.ToString() }
			};

			if (!string.IsNullOrWhiteSpace(idDocumento))
			{
				queryParams["idDocumento"] = idDocumento;
			}

			if (!string.IsNullOrWhiteSpace(nombreDocumento))
			{
				queryParams["nombreDocumento"] = nombreDocumento;
			}

			return queryParams;
		}
	}
}
