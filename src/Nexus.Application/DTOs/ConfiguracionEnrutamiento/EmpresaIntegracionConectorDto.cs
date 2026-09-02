namespace Nexus.Application.DTOs.ConfiguracionEnrutamiento;

public class EmpresaIntegracionConectorDto
{
    public long Id { get; set; }
    public long EmpresaId { get; set; }
    public long IntegracionConectorId { get; set; }
    public long IntegracionId { get; set; }
    public string? NombreIntegracion { get; set; }
    public string? CodigoAccion { get; set; }
    public long ConectorId { get; set; }
    public string? NombreConector { get; set; }
    public string? UrlBaseConector { get; set; }
    public string? RutaEndpoint { get; set; }
    public string? ColaRabbitMQDestino { get; set; }
}
