namespace Nexus.Domain.Enums
{
    public enum TipoIntegracion
    {
        Consulta = 1, // HTTP / REST (Sincrónico)
        Escritura = 2 // RabbitMQ / Queue (Asincrónico)
    }
}
