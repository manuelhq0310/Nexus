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
    public TipoIntegracion? Tipo { get; set; }
    public bool EsAsincrono { get; set; }
    public bool Estado { get; set; }
}

public class CrearIntegracionDto
{
    public string CodigoAccion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public TipoIntegracion? Tipo { get; set; }

    /// <summary>true: se enruta por la cola de RabbitMQ; false: por la RutaEndpoint (HTTP).</summary>
    public bool EsAsincrono { get; set; }
}

public class ActualizarIntegracionDto
{
    public string CodigoAccion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public TipoIntegracion? Tipo { get; set; }

    /// <summary>true: se enruta por la cola de RabbitMQ; false: por la RutaEndpoint (HTTP).</summary>
    public bool EsAsincrono { get; set; }
}
