using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nexus.Domain.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Nexus.Infrastructure.Messaging
{
    public class RabbitMqMessageBrokerPublisher : IMessageBrokerPublisher
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMqMessageBrokerPublisher> _logger;

        public RabbitMqMessageBrokerPublisher(
                        IConfiguration configuration,
                        ILogger<RabbitMqMessageBrokerPublisher> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task PublishAsync<T>(string routingKey, T message) where T : class
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                Port = int.TryParse(_configuration["RabbitMQ:Port"], out var port) ? port : 5672,
                UserName = _configuration["RabbitMQ:Username"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest"
            };

            try
            {
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                string exchangeName = "delayed-exchange";

                // 1. Declarar el exchange diferido
                await channel.ExchangeDeclareAsync(
                    exchange: exchangeName,
                    type: "x-delayed-message",
                    durable: true,
                    autoDelete: false,
                    arguments: new Dictionary<string, object?>
                    {
                { "x-delayed-type", "direct" }
                    });

                // 2. Declarar la cola
                await channel.QueueDeclareAsync(
                    queue: routingKey,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                // 3. VINCULAR LA COLA AL EXCHANGE (Paso crítico que faltaba)
                await channel.QueueBindAsync(
                    queue: routingKey,
                    exchange: exchangeName,
                    routingKey: routingKey,
                    arguments: null
                );

                var jsonMessage = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                var basicProperties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json",
                    Headers = new Dictionary<string, object?>
                    {
                        { "x-delay", 1000 } // El tiempo de retraso debe estar en milisegundos (1000ms = 1s)
                    }
                };

                await channel.BasicPublishAsync(
                    exchange: exchangeName,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: basicProperties,
                    body: body
                );

                _logger.LogInformation(
                    "Mensaje publicado exitosamente en RabbitMQ. Exchange: '{Exchange}', RoutingKey: '{RoutingKey}'",
                    exchangeName,
                    routingKey
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al publicar el mensaje en RabbitMQ. Exchange: '{Exchange}', RoutingKey: '{RoutingKey}'",
                    "delayed-exchange",
                    routingKey
                );
                throw;
            }
        }
    }
}
