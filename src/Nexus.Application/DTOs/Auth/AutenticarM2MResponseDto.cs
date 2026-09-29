namespace Nexus.Application.DTOs.Auth
{
    public class AutenticarAplicacionResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAt { get; set; }
    }
}
