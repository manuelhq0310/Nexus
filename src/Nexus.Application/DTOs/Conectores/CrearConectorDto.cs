using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.Conectores;

public class CrearConectorDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de protocolo es obligatorio.")]
    [MaxLength(20)]
    public string TipoProtocolo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La URL base es obligatoria.")]
    [MaxLength(250)]
    [Url(ErrorMessage = "Debe ser una URL válida.")]
    public string UrlBase { get; set; } = string.Empty;
}
