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

        public IntegracionRouterService(
            IAplicacionEmpresaRepository aplicacionEmpresaRepository,
            IAplicacionIntegracionRepository aplicacionIntegracionRepository,
            IHttpClientFactory httpClientFactory,
            IMessageBrokerPublisher messagePublisher,
            IUnoEConsultaService unoEConsultaService,
            IIntgRabbitmqRequestRepository rabbitmqRequestRepo)
        {
            _aplicacionEmpresaRepository = aplicacionEmpresaRepository;
            _aplicacionIntegracionRepository = aplicacionIntegracionRepository;
            _httpClientFactory = httpClientFactory;
            _messagePublisher = messagePublisher;
            _unoEConsultaService = unoEConsultaService;
            _rabbitmqRequestRepo = rabbitmqRequestRepo;
        }

        public async Task<EjecutarIntegracionResponse> ProcesarIntegracionAsync(EjecutarIntegracionRequest request)
        {
            // 1. Validar que la Empresa tenga asignada la Aplicación y traer la entidad Empresa con su Conector
            var aplicacionEmpresa = await _aplicacionEmpresaRepository
                .ObtenerConConectorPorCodigosAsync(request.CodigoAplicacion, request.CodigoEmpresa);

            if (aplicacionEmpresa == null || !aplicacionEmpresa.Estado)
            {
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
                return new EjecutarIntegracionResponse(
                    false,
                    $"La aplicación '{aplicacionIntegracion.Aplicacion.Nombre}' no está habilitada para el conector '{empresaConector.Conector.Nombre}'.",
                    null,
                    null
                );
            }

            if (integracion.Tipo == TipoIntegracion.Escritura)
            {
                // Construcción del nombre de la cola según el conector de la empresa
                string nombreCola = integracionConector.ColaRabbitMQDestino ?? string.Empty;
                return await PublicarAColaAsync(nombreCola, request);
            }
            else
            {
                // Construcción de la URL endpoint combinando la BaseUrl del Conector con el Path de la Integración
                string urlEndpoint = $"{empresaConector.Conector.UrlBase.TrimEnd('/')}/{integracionConector.RutaEndpoint.TrimStart('/')}";
                object payload;
                if (integracion.ConsultaGenerica)
                {
                    
                    payload = await _unoEConsultaService.PrepararConsultaGenerica(request, aplicacionConector);
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
            var client = _httpClientFactory.CreateClient("ConectoresClient");
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            response = await client.PostAsync(urlEndpoint, content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var dataResult = JsonSerializer.Deserialize<object>(responseBody);
                return new EjecutarIntegracionResponse(true, "Integración ejecutada en el conector con éxito.", dataResult, Guid.NewGuid().ToString());
            }

            return new EjecutarIntegracionResponse(false, $"Error devuelto por el conector ({response.StatusCode}):", responseBody, null);
        }

        private async Task<EjecutarIntegracionResponse> PublicarAColaAsync(
            string nombreCola,
            EjecutarIntegracionRequest request)
        {
            var transaccionId = Guid.NewGuid().ToString();

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

            return new EjecutarIntegracionResponse(
                true,
                $"Mensaje publicado en la cola '{nombreCola}' para procesamiento en el conector.",
                null,
                transaccionId
            );
        }
    }
}
