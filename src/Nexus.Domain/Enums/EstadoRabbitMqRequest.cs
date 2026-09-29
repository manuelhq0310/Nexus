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
        /// La petición se envió con éxito a UnoE, pero respondió con errores.
        /// </summary>
        ProcesadaConErrores = 3,

        /// <summary>
        /// Ocurrió un error de sistema en el consumo o durante la ejecución en UnoE.
        /// </summary>
        Fallida = 4
    }
}
