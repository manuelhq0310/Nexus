namespace Nexus.Presentation.Models;

/// <summary>Relación Integración &lt;-&gt; Conector (qué conector resuelve qué integración y con qué endpoint).</summary>
public class IntegracionConectorDto
{
    public long Id { get; set; }
    public long IntegracionId { get; set; }
    public string? NombreIntegracion { get; set; }
    public string? CodigoAccion { get; set; }
    public long ConectorId { get; set; }
    public string? NombreConector { get; set; }
    public string RutaEndpoint { get; set; } = string.Empty;
    public string? ColaRabbitMQDestino { get; set; }
    public bool Estado { get; set; }
}

public class CrearIntegracionConectorDto
{
    public long IntegracionId { get; set; }
    public long ConectorId { get; set; }
    public string RutaEndpoint { get; set; } = string.Empty;
    public string? ColaRabbitMQDestino { get; set; }
}

public class ActualizarIntegracionConectorDto
{
    public string RutaEndpoint { get; set; } = string.Empty;
    public string? ColaRabbitMQDestino { get; set; }
}

/// <summary>
/// Matriz final de enrutamiento: qué Empresa usa qué combinación Integración-Conector,
/// con sus credenciales/config de autenticación propias (endpoint "ConfiguracionEnrutamiento").
/// </summary>
public class EmpresaIntegracionConectorDto
{
    public long Id { get; set; }
    public long EmpresaId { get; set; }
    public long IntegracionConectorId { get; set; }
    public long IntegracionId { get; set; }
    public string? NombreIntegracion { get; set; }
    public string? CodigoAccion { get; set; }
    public long ConectorId { get; set; }
    public string? NombreConector { get; set; }
    public string? UrlBaseConector { get; set; }
    public string? RutaEndpoint { get; set; }
    public string? ColaRabbitMQDestino { get; set; }
}

public class CrearEmpresaIntegracionConectorDto
{
    public long EmpresaId { get; set; }
    public long IntegracionConectorId { get; set; }
    public bool RequiereAutenticacion { get; set; }
    public string? ApiKey { get; set; }
    public string? TokenBearer { get; set; }
}

public class ActualizarEmpresaIntegracionConectorDto
{
    public bool RequiereAutenticacion { get; set; }
    public string? ApiKey { get; set; }
    public string? TokenBearer { get; set; }
    public bool Activo { get; set; }
}

/// <summary>Resultado de resolver dinámicamente la ruta de enrutamiento para una empresa + acción.</summary>
public class RutaEnrutamientoResueltaDto
{
    public long EmpresaId { get; set; }
    public string? CodigoAccion { get; set; }
    public string? ProtocoloConector { get; set; }
    public string? UrlBaseConector { get; set; }
    public string? RutaEndpoint { get; set; }
    public string? UrlCompleta { get; set; }
    public string? ColaRabbitMQDestino { get; set; }
}
