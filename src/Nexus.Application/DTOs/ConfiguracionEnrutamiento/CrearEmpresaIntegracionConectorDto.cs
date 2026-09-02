using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.ConfiguracionEnrutamiento;

public class CrearEmpresaIntegracionConectorDto
{
    [Required(ErrorMessage = "La empresa es obligatoria.")]
    public long EmpresaId { get; set; }

    [Required(ErrorMessage = "La relación integración-conector es obligatoria.")]
    public long IntegracionConectorId { get; set; }

    public bool RequiereAutenticacion { get; set; }
    public string? ApiKey { get; set; }
    public string? TokenBearer { get; set; }
}
