namespace Nexus.Presentation.Models;

public class EmpresaDto
{
    public long Id { get; set; }
    public string TipoIdentificacion { get; set; } = string.Empty;
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string NombreRazonSocial { get; set; } = string.Empty;
    public bool Estado { get; set; }
}

public class CrearEmpresaDto
{
    public string TipoIdentificacion { get; set; } = string.Empty;
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string NombreRazonSocial { get; set; } = string.Empty;
}

public class ActualizarEmpresaDto
{
    public string TipoIdentificacion { get; set; } = string.Empty;
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string NombreRazonSocial { get; set; } = string.Empty;
}

/// <summary>Relación directa Empresa &lt;-&gt; Conector (qué conector usa cada empresa).</summary>
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

public class AsignarEmpresaConectorDto
{
    public long EmpresaId { get; set; }
    public long ConectorId { get; set; }
}
