namespace Nexus.Application.DTOs.AplicacionConectores;

public class AplicacionConectorDto
{
    public long Id { get; set; }
    public long AplicacionId { get; set; }
    public string? NombreAplicacion { get; set; }
    public string? CodigoApp { get; set; }
    public long ConectorId { get; set; }
    public string? NombreConector { get; set; }
    public string? UsuarioErp { get; set; }
    public bool Estado { get; set; }
}
