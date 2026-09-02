namespace Nexus.Application.DTOs.Empresas;

public class EmpresaDto
{
    public long Id { get; set; }
    public string TipoIdentificacion { get; set; } = string.Empty;
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string NombreRazonSocial { get; set; } = string.Empty;
    public bool Estado { get; set; }
}
