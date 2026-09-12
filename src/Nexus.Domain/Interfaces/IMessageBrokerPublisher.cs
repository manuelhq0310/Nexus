namespace Nexus.Domain.Interfaces
{
    public interface IMessageBrokerPublisher
    {
        Task PublishAsync<T>(string routingKey, T message) where T : class;
    }
}
