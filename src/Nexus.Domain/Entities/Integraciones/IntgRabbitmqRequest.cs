using Nexus.Domain.Enums;

namespace Nexus.Domain.Entities.Integraciones
{
    public class IntgRabbitmqRequest
    {
        public string RabbitmqRequestId { get; set; } = string.Empty;
        public string CodigoAccionIntegracion { get; set; } = string.Empty;
        public int CodigoEmpresa { get; set; }
        public string ColaRabbitMqDestino { get; set; } = string.Empty;
        public EstadoRabbitMqRequest Estado { get; set; } = EstadoRabbitMqRequest.Pendiente;
        public string? Payload { get; set; }
        public int Intentos { get; set; } = 0;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaProcesado { get; set; }
        public string? RespuestaDetalle { get; set; }
        public string Usuario { get; set; } = string.Empty;
    }
}
