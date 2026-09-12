namespace Nexus.Application.DTOs.UnoEConsultaConfig
{
    // Request que envía el cliente (ej. Tickelia) al endpoint de Nexus
    public class EjecutarConsultaRequestDto
    {
        public string CodigoConsulta { get; set; } = string.Empty;
        public string CodigoApp { get; set; } = string.Empty; // Corresponde a IntgAplicacion.CodigoApp
        public string NumeroIdentificacionEmpresa { get; set; } = string.Empty; // Corresponde a IntgEmpresas.NumeroIdentificacion
        public string ParametrosXml { get; set; } = string.Empty; // XML de parámetros
    }

    // Request enviado al cliente del Conector UnoE
    public class ConsultaGeneralRequest
    {
        public string? NombreConexion { get; set; }
        public string? IdCia { get; set; }
        public string? IdProveedor { get; set; }
        public string? IdConsulta { get; set; }
        public string? Usuario { get; set; }
        public string? Clave { get; set; }
        public string? Parametros { get; set; }
    }
}
