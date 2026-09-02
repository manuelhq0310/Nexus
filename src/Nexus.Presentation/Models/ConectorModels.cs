namespace Nexus.Presentation.Models;

public class ConectorDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string TipoProtocolo { get; set; } = string.Empty;
    public string UrlBase { get; set; } = string.Empty;
    public bool Estado { get; set; }
}

public class CrearConectorDto
{
    public string Nombre { get; set; } = string.Empty;
    public string TipoProtocolo { get; set; } = string.Empty;
    public string UrlBase { get; set; } = string.Empty;
}

public class ActualizarConectorDto
{
    public string Nombre { get; set; } = string.Empty;
    public string TipoProtocolo { get; set; } = string.Empty;
    public string UrlBase { get; set; } = string.Empty;
}
