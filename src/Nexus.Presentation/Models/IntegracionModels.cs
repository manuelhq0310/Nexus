namespace Nexus.Presentation.Models;

public class IntegracionDto
{
    public long Id { get; set; }
    public string CodigoAccion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Estado { get; set; }
}

public class CrearIntegracionDto
{
    public string CodigoAccion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class ActualizarIntegracionDto
{
    public string CodigoAccion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
