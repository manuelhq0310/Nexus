using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.Application.DTOs.IntegracionRouter;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Domain.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Nexus.Infrastructure.Messaging
{
    public class UnoERespuestasConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<UnoERespuestasConsumer> _logger;
        private readonly IConfiguration _configuration;
        private const string QueueName = "fn-conector-unoee-respuestas";

        public UnoERespuestasConsumer(
            IServiceScopeFactory scopeFactory,
            ILogger<UnoERespuestasConsumer> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                Port = int.TryParse(_configuration["RabbitMQ:Port"], out var port) ? port : 5672,
                UserName = _configuration["RabbitMQ:Username"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest"
            };

            // 1. Crear el canal asíncronamente
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // 2. Declarar la cola asíncronamente
            await channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken
            );

            // 3. Crear el consumidor asíncrono
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Mensaje recibido de la cola '{Queue}': {Json}", QueueName, messageJson);

                try
                {
                    var respuesta = JsonSerializer.Deserialize<RespuestaMensajeRabbit>(messageJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (respuesta != null && !string.IsNullOrWhiteSpace(respuesta.RequestId))
                    {
                        await ProcesarRespuestaAsync(respuesta);
                        await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                    }
                    else
                    {
                        _logger.LogWarning("Respuesta recibida en {Queue} con formato o RequestId inválido.", QueueName);
                        await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando el mensaje de respuesta de UnoE. DeliveryTag: {Tag}", ea.DeliveryTag);
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                }
            };

            await channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

            // Mantener el Worker corriendo mientras no se solicite cancelación
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcesarRespuestaAsync(RespuestaMensajeRabbit respuesta)
        {
            using var scope = _scopeFactory.CreateScope();
            var rabbitmqRequestRepo = scope.ServiceProvider.GetRequiredService<IIntgRabbitmqRequestRepository>();

            var resultado = respuesta.Resultado;
            var estadoFinal = resultado.Exitoso == null ? 
                              EstadoRabbitMqRequest.Fallida : 
                              (resultado.Exitoso == true ? EstadoRabbitMqRequest.Completada : EstadoRabbitMqRequest.ProcesadaConErrores);

            string detalleRespuesta = resultado.Exitoso == true
                ? $"Transacción exitosa."
                : $"Servicio: {respuesta.TipoServicio}. Error: {resultado.MensajeError}";

            await rabbitmqRequestRepo.UpdateEstadoAsync(
                id: respuesta.RequestId,
                estado: estadoFinal,
                respuestaDetalle: detalleRespuesta,
                fechaProcesado: DateTime.UtcNow
            );
        }
    }
}
