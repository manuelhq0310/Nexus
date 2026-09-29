namespace Nexus.Application.DTOs.Aplicaciones
{
    public class IniciarOnboardingResponseDto
    {
        public string CodigoAplicacion { get; set; } = string.Empty;
        public string OnboardingToken { get; set; } = string.Empty;
        public DateTime FechaExpiracionUtc { get; set; }
        public string Mensaje { get; set; } = "Entregue este token y el código de aplicación al equipo del tercero para que reclame sus credenciales mediante el endpoint de claim-credentials.";
    }
}
