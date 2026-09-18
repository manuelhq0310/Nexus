using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.Empresas;

public class ActualizarEmpresaDto
{
    [Required(ErrorMessage = "El código de empresa es obligatorio.")]
    public int CodigoEmpresa { get; set; }

    [Required(ErrorMessage = "La razón social es obligatoria.")]
    [MaxLength(150)]
    public string NombreRazonSocial { get; set; } = string.Empty;
}
