namespace Nexus.Application.DTOs.Aplicaciones
{
    public class IniciarOnboardingRequestDto
    {
        public string CodigoAplicacion { get; set; } = string.Empty;
        public int HorasVigencia { get; set; } = 24;
    }    
}
