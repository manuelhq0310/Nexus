namespace Nexus.Application.DTOs.Empresas;

public class EmpresaDto
{
    public long Id { get; set; }
    public int CodigoEmpresa { get; set; }
    public string NombreRazonSocial { get; set; } = string.Empty;
    public bool Estado { get; set; }
}
