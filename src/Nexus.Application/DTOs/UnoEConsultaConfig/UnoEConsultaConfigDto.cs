namespace Nexus.Application.DTOs.UnoEConsultaConfig
{
    // Request enviado al cliente del Conector UnoE
    public class ConsultaGeneralRequest
    {
        public string? NombreConexion { get; set; }
        public int IdCia { get; set; }
        public string? IdProveedor { get; set; }
        public string? IdConsulta { get; set; }
        public string? Usuario { get; set; }
        public string? Clave { get; set; }
        public string? Parametros { get; set; }
    }
}
