namespace Nexus.Application.DTOs.Aplicaciones
{
    public class ReclamarCredencialesResponseDto
    {
        public string NombreAplicacion { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Mensaje { get; set; } = "Guarde este ClientSecret inmediatamente en un almacenamiento seguro (gestor de secretos o variables de entorno). Por motivos de seguridad no se podrá volver a consultar ni recuperar.";
        public DateTime FechaGeneracionUtc { get; set; } = DateTime.UtcNow;
    }
}
