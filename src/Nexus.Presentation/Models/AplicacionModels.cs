namespace Nexus.Presentation.Models;

public class AplicacionDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string CodigoApp { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Estado { get; set; }
}

public class CrearAplicacionDto
{
    public string Nombre { get; set; } = string.Empty;
    public string CodigoApp { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class ActualizarAplicacionDto
{
    public string Nombre { get; set; } = string.Empty;
    public string CodigoApp { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

/// <summary>Relación Aplicación &lt;-&gt; Integración (clave compuesta, sin Id propio).</summary>
public class AplicacionIntegracionDto
{
    public long AplicacionId { get; set; }
    public string? NombreAplicacion { get; set; }
    public long IntegracionId { get; set; }
    public string? NombreIntegracion { get; set; }
    public string? DescripcionIntegracion { get; set; }
    public bool Estado { get; set; }
}

public class CrearAplicacionIntegracionDto
{
    public long AplicacionId { get; set; }
    public long IntegracionId { get; set; }
}

/// <summary>Relación Aplicación &lt;-&gt; Empresa (empresas que utilizan la aplicación).</summary>
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

public class AsignarEmpresaAAplicacionDto
{
    public long AplicacionId { get; set; }
    public long EmpresaId { get; set; }
}

/// <summary>Relación Aplicación &lt;-&gt; Conector, con las credenciales que la aplicación usa para autenticarse.</summary>
public class AplicacionConectorDto
{
    public long Id { get; set; }
    public long AplicacionId { get; set; }
    public string? NombreAplicacion { get; set; }
    public string? CodigoApp { get; set; }
    public long ConectorId { get; set; }
    public string? NombreConector { get; set; }
    public string? UsuarioErp { get; set; }
    public bool Estado { get; set; }
}

public class CrearAplicacionConectorDto
{
    public long AplicacionId { get; set; }
    public long ConectorId { get; set; }
    public string? UrlBasePersonalizada { get; set; }
    public string? UsuarioErp { get; set; }
    public string? PasswordErp { get; set; }
    public string? ApiKey { get; set; }
    public string? TokenBearer { get; set; }
}

public class ActualizarAplicacionConectorDto
{
    public string? UrlBasePersonalizada { get; set; }
    public string? UsuarioErp { get; set; }
    public string? PasswordErp { get; set; }
    public string? ApiKey { get; set; }
    public string? TokenBearer { get; set; }
    public bool Estado { get; set; }
}
