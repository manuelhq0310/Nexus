using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.Empresas;

public class ActualizarEmpresaDto
{
    [Required(ErrorMessage = "El tipo de identificación es obligatorio.")]
    [MaxLength(10)]
    public string TipoIdentificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de identificación es obligatorio.")]
    [MaxLength(20)]
    public string NumeroIdentificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La razón social es obligatoria.")]
    [MaxLength(150)]
    public string NombreRazonSocial { get; set; } = string.Empty;
}
