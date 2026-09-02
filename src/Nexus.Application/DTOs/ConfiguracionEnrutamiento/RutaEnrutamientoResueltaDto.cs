namespace Nexus.Application.DTOs.ConfiguracionEnrutamiento;

/// <summary>Resultado de resolver dinámicamente, en tiempo de ejecución, la ruta de enrutamiento para una empresa + acción.</summary>
public class RutaEnrutamientoResueltaDto
{
    public long EmpresaId { get; set; }
    public string? CodigoAccion { get; set; }
    public string? ProtocoloConector { get; set; }
    public string? UrlBaseConector { get; set; }
    public string? RutaEndpoint { get; set; }
    public string? UrlCompleta { get; set; }
    public string? ColaRabbitMQDestino { get; set; }
}
