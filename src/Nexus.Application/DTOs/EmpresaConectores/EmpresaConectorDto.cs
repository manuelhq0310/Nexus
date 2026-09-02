namespace Nexus.Application.DTOs.EmpresaConectores;

public class EmpresaConectorDto
{
    public long Id { get; set; }
    public long EmpresaId { get; set; }
    public long ConectorId { get; set; }
    public string? NombreConector { get; set; }
    public string? TipoProtocolo { get; set; }
    public string? UrlBase { get; set; }
    public bool Estado { get; set; }
}
