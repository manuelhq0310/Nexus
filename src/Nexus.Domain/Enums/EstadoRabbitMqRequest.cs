namespace Nexus.Domain.Enums
{
    public enum EstadoRabbitMqRequest
    {
        /// <summary>
        /// El mensaje fue publicado exitosamente en la cola de RabbitMQ.
        /// </summary>
        Pendiente = 1,

        /// <summary>
        /// La transacción se completó exitosamente en el ERP/servicio de destino.
        /// </summary>
        Completada = 2,

        /// <summary>
        /// Ocurrió un error en  el consumo o durante la ejecución en UnoE.
        /// </summary>
        Fallida = 3
    }
}
