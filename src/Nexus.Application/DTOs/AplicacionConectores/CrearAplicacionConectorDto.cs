using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.AplicacionConectores;

public class CrearAplicacionConectorDto
{
    [Required(ErrorMessage = "La aplicación es obligatoria.")]
    public long AplicacionId { get; set; }

    [Required(ErrorMessage = "El conector es obligatorio.")]
    public long ConectorId { get; set; }

    public string? UrlBasePersonalizada { get; set; }
    public string? UsuarioErp { get; set; }
    public string? PasswordErp { get; set; }
    public string? ApiKey { get; set; }
    public string? TokenBearer { get; set; }
}
