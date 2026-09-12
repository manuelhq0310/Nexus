using Nexus.Domain.Enums;

namespace Nexus.Application.DTOs.Integraciones;

public class IntegracionDto
{
    public long Id { get; set; }
    public string CodigoAccion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public TipoIntegracion Tipo { get; set; }
    public bool ConsultaGenerica { get; set; }
    public bool Estado { get; set; }
}
