namespace Nexus.Application.DTOs.AplicacionEmpresas;

public class AplicacionEmpresaDto
{
    public long Id { get; set; }
    public long AplicacionId { get; set; }
    public string? NombreAplicacion { get; set; }
    public string? CodigoAplicacion { get; set; }
    public long EmpresaId { get; set; }
    public string? NombreEmpresa { get; set; }
    public bool Estado { get; set; }
}
