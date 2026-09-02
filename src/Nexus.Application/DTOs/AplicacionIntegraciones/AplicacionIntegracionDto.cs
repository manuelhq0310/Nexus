namespace Nexus.Application.DTOs.AplicacionIntegraciones;

public class AplicacionIntegracionDto
{
    public long AplicacionId { get; set; }
    public string? NombreAplicacion { get; set; }
    public long IntegracionId { get; set; }
    public string? NombreIntegracion { get; set; }
    public string? DescripcionIntegracion { get; set; }
    public bool Estado { get; set; }
}
