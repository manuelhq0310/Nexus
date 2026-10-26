using Microsoft.Extensions.Logging;
using Nexus.Application.DTOs.IntegracionRouter;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Domain.Enums;
using Nexus.Domain.Interfaces;
using System.Text.Json;

namespace Nexus.Application.Services
{
	public class IntegracionRouterService : IIntegracionRouterService
	{
		private readonly IAplicacionEmpresaRepository _aplicacionEmpresaRepository;
		private readonly IAplicacionIntegracionRepository _aplicacionIntegracionRepository;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IMessageBrokerPublisher _messagePublisher;
		private readonly IUnoEConsultaService _unoEConsultaService;
		private readonly IIntgRabbitmqRequestRepository _rabbitmqRequestRepo;
		private readonly ILogger<IntegracionRouterService> _logger;

		public IntegracionRouterService(
			IAplicacionEmpresaRepository aplicacionEmpresaRepository,
			IAplicacionIntegracionRepository aplicacionIntegracionRepository,
			IHttpClientFactory httpClientFactory,
			IMessageBrokerPublisher messagePublisher,
			IUnoEConsultaService unoEConsultaService,
			IIntgRabbitmqRequestRepository rabbitmqRequestRepo,
			ILogger<IntegracionRouterService> logger)
		{
			_aplicacionEmpresaRepository = aplicacionEmpresaRepository;
			_aplicacionIntegracionRepository = aplicacionIntegracionRepository;
			_httpClientFactory = httpClientFactory;
			_messagePublisher = messagePublisher;
			_unoEConsultaService = unoEConsultaService;
			_rabbitmqRequestRepo = rabbitmqRequestRepo;
			_logger = logger;
		}		

		public async Task<EjecutarIntegracionResponse> ProcesarIntegracionAsync(EjecutarIntegracionRequest request)
		{
			_logger.LogInformation("Procesando integración '{CodigoIntegracion}' para la aplicación '{CodigoAplicacion}' y empresa '{CodigoEmpresa}'",
				request.CodigoIntegracion, request.CodigoAplicacion, request.CodigoEmpresa);

			// 1. Validar que la Empresa tenga asignada la Aplicación y traer la entidad Empresa con su Conector
			var aplicacionEmpresa = await _aplicacionEmpresaRepository
				.ObtenerConConectorPorCodigosAsync(request.CodigoAplicacion, request.CodigoEmpresa);

			if (aplicacionEmpresa == null || !aplicacionEmpresa.Estado)
			{
				_logger.LogWarning("Validación fallida: La empresa '{CodigoEmpresa}' no tiene suscripción activa a la aplicación '{CodigoAplicacion}' (o no existe).",
					request.CodigoEmpresa, request.CodigoAplicacion);

				return new EjecutarIntegracionResponse(
					false,
					$"La empresa '{request.CodigoEmpresa}' no tiene suscripción activa a la aplicación '{request.CodigoAplicacion}'.",
					null,
					null
				);
			}

			var empresaConector = aplicacionEmpresa.Empresa?.EmpresaConector;
			if (empresaConector == null || !empresaConector.Estado)
			{
				_logger.LogWarning("Validación fallida: La empresa '{CodigoEmpresa}' no tiene un conector activo configurado.", request.CodigoEmpresa);

				return new EjecutarIntegracionResponse(
					false,
					$"La empresa '{request.CodigoEmpresa}' no tiene un conector activo configurado.",
					null,
					null
				);
			}

			// 2. Validar que la Integración esté asociada a la Aplicación
			var aplicacionIntegracion = await _aplicacionIntegracionRepository
				.ObtenerPorCodigosAsync(request.CodigoAplicacion, request.CodigoIntegracion);

			if (aplicacionIntegracion == null || !aplicacionIntegracion.Estado)
			{
				_logger.LogWarning("Validación fallida: La integración '{CodigoIntegracion}' no está habilitada para la aplicación '{CodigoAplicacion}'.",
					request.CodigoIntegracion, request.CodigoAplicacion);

				return new EjecutarIntegracionResponse(
					false,
					$"La integración '{request.CodigoIntegracion}' no está habilitada para la aplicación '{request.CodigoAplicacion}'.",
					null,
					null
				);
			}

			// 3. Determinar vía de envío y resolver ruta dinámica
			var integracion = aplicacionIntegracion.Integracion;
			var integracionConector = integracion.IntegracionConectores.FirstOrDefault(i => i.ConectorId == empresaConector.ConectorId);
			if (integracionConector == null || !integracionConector.Estado)
			{
				_logger.LogWarning("Validación fallida: La integración '{CodigoIntegracion}' no está habilitada para el conector ID '{ConectorId}'.",
					request.CodigoIntegracion, empresaConector.ConectorId);

				return new EjecutarIntegracionResponse(
					false,
					$"La integración '{request.CodigoIntegracion}' no está habilitada para el conector '{empresaConector.Conector.Nombre}'.",
					null,
					null
				);
			}

			var aplicacionConector = aplicacionIntegracion.Aplicacion.AplicacionConectores.FirstOrDefault(a => a.ConectorId == empresaConector.ConectorId);
			if (aplicacionConector == null || !aplicacionConector.Estado)
			{
				_logger.LogWarning("Validación fallida: La aplicación '{NombreAplicacion}' no está habilitada para el conector ID '{ConectorId}'.",
					aplicacionIntegracion.Aplicacion.Nombre, empresaConector.ConectorId);

				return new EjecutarIntegracionResponse(
					false,
					$"La aplicación '{aplicacionIntegracion.Aplicacion.Nombre}' no está habilitada para el conector '{empresaConector.Conector.Nombre}'.",
					null,
					null
				);
			}

			if (integracion.EsAsincrono)
			{
				string nombreCola = integracionConector.ColaRabbitMQDestino ?? string.Empty;
				_logger.LogInformation("La integración '{CodigoIntegracion}' está configurada como ASÍNCRONA. Derivando a cola: {NombreCola}",
					request.CodigoIntegracion, nombreCola);

				return await PublicarAColaAsync(nombreCola, request);
			}
			else
			{
				string urlEndpoint = $"{empresaConector.Conector.UrlBase.TrimEnd('/')}/{integracionConector.RutaEndpoint!.TrimStart('/')}";
				_logger.LogInformation("La integración '{CodigoIntegracion}' está configurada como SÍNCRONA. Endpoint destino: {UrlEndpoint}",
					request.CodigoIntegracion, urlEndpoint);

				object payload;
				if (integracionConector.EsConsultaGenerica)
				{
					_logger.LogDebug("Preparando consulta genérica para la integración '{CodigoIntegracion}'", request.CodigoIntegracion);
					payload = await _unoEConsultaService.PrepararConsultaGenerica(request, aplicacionConector, integracionConector);
				}
				else
				{
					payload = request.Payload;
				}

				return await EnviarAEndpointHttpAsync(urlEndpoint, payload);
			}
		}

		private async Task<EjecutarIntegracionResponse> EnviarAEndpointHttpAsync(
			string urlEndpoint,
			object payload)
		{
			_logger.LogInformation("Enviando petición HTTP POST a Endpoint: {UrlEndpoint}", urlEndpoint);

			var client = _httpClientFactory.CreateClient("ConectoresClient");
			var jsonPayload = JsonSerializer.Serialize(payload);
			var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

			HttpResponseMessage response;

			try
			{
				response = await client.PostAsync(urlEndpoint, content);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Excepción de red/HTTP al intentar conectar con el endpoint {UrlEndpoint}", urlEndpoint);
				throw;
			}

			var responseBody = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation("Petición HTTP exitosa hacia {UrlEndpoint}. StatusCode: {StatusCode}", urlEndpoint, response.StatusCode);

				var dataResult = JsonSerializer.Deserialize<object>(responseBody);
				return new EjecutarIntegracionResponse(true, "Integración ejecutada en el conector con éxito.", dataResult, Guid.NewGuid().ToString());
			}

			_logger.LogWarning("El conector respondió con error HTTP {StatusCode} en {UrlEndpoint}. Respuesta: {ResponseBody}",
				response.StatusCode, urlEndpoint, responseBody);

			return new EjecutarIntegracionResponse(false, $"Error devuelto por el conector.", responseBody, null);
		}

		private async Task<EjecutarIntegracionResponse> PublicarAColaAsync(
			string nombreCola,
			EjecutarIntegracionRequest request)
		{
			var transaccionId = Guid.NewGuid().ToString();

			_logger.LogInformation("Publicando mensaje en broker RabbitMQ [Cola: {NombreCola}] con TransactionId: {TransaccionId}",
				nombreCola, transaccionId);

			await _messagePublisher.PublishAsync(
				routingKey: nombreCola,
				message: new
				{
					RequestId = transaccionId,
					request.CodigoAplicacion,
					request.CodigoEmpresa,
					request.CodigoIntegracion,
					request.Payload
				}
			);

			// Crear el registro inicial de la traza (Estado: Pendiente)
			var rabbitRequest = new IntgRabbitmqRequest
			{
				RabbitmqRequestId = transaccionId,
				CodigoAccionIntegracion = request.CodigoIntegracion,
				CodigoEmpresa = request.CodigoEmpresa,
				ColaRabbitMqDestino = nombreCola,
				Estado = EstadoRabbitMqRequest.Pendiente,
				Payload = request.Payload.ToString(),
				Intentos = 0,
				FechaCreacion = DateTime.UtcNow,
				Usuario = "Default"
			};

			await _rabbitmqRequestRepo.AddAsync(rabbitRequest);

			_logger.LogInformation("Registro de traza RabbitMQ guardado exitosamente con ID: {TransaccionId}", transaccionId);

			return new EjecutarIntegracionResponse(
				true,
				$"Mensaje publicado en la cola '{nombreCola}' para procesamiento en el conector.",
				null,
				transaccionId
			);
		}
	}
}
