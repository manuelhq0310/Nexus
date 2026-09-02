namespace Nexus.Application.DTOs.IntegracionConectores;

public class IntegracionConectorDto
{
    public long Id { get; set; }
    public long IntegracionId { get; set; }
    public string? NombreIntegracion { get; set; }
    public string? CodigoAccion { get; set; }
    public long ConectorId { get; set; }
    public string? NombreConector { get; set; }
    public string RutaEndpoint { get; set; } = string.Empty;
    public string? ColaRabbitMQDestino { get; set; }
    public bool Estado { get; set; }
}
