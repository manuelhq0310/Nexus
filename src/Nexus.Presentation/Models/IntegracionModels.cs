namespace Nexus.Presentation.Models;

/// <summary>Coincide con el enum TipoIntegracion del backend (valores enteros 1 y 2).</summary>
public enum TipoIntegracion
{
    Consulta = 1,
    Escritura = 2
}

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

    /// <summary>Solo aplica al crear: el backend no lo expone en IntegracionDto ni en ActualizarIntegracionDto.</summary>
    public TipoIntegracion? Tipo { get; set; }

    /// <summary>Solo tiene sentido cuando Tipo = Consulta.</summary>
    public bool ConsultaGenerica { get; set; }
}

public class ActualizarIntegracionDto
{
    public string CodigoAccion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
