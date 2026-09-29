namespace Nexus.Application.DTOs.Auth
{
    public class AutenticarAplicacionRequestDto
    {
        public string CodigoAplicacion { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}
