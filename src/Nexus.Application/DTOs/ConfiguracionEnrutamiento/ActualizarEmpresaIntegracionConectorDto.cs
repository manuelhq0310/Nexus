namespace Nexus.Application.DTOs.ConfiguracionEnrutamiento;

public class ActualizarEmpresaIntegracionConectorDto
{
    public bool RequiereAutenticacion { get; set; }
    public string? ApiKey { get; set; }
    public string? TokenBearer { get; set; }
    public bool Activo { get; set; }
}
