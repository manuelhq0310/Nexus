namespace Nexus.Application.DTOs.IntegracionRouter
{
    public class RespuestaMensajeRabbit
    {
        public string RequestId { get; set; } = string.Empty;
        public string TipoServicio { get; set; } = string.Empty;
        public ResultadoRespuesta Resultado { get; set; } = new();
    }

    public class ResultadoRespuesta
    {
        public bool? Exitoso { get; set; }
        public string? ConsecutivoDocumento { get; set; }
        public string? TipoDocumento { get; set; }
        public string? MensajeError { get; set; }
        public List<string> DetalleErrores { get; set; } = new();
    }
}
