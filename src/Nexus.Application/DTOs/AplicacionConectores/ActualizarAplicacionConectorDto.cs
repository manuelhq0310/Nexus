namespace Nexus.Application.DTOs.AplicacionConectores;

public class ActualizarAplicacionConectorDto
{
    public string? UrlBasePersonalizada { get; set; }
    public string? UsuarioErp { get; set; }
    public string? PasswordErp { get; set; }
    public string? ApiKey { get; set; }
    public string? TokenBearer { get; set; }
    public bool Estado { get; set; }
}
