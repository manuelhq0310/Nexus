namespace Nexus.Application.DTOs.Integraciones;

public class IntegracionDto
{
    public long Id { get; set; }
    public string CodigoAccion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Estado { get; set; }
}
