namespace Nexus.Domain.Entities.Integraciones
{
    public class UnoEConsultaConfig
    {
        public long Id { get; set; }
        public string CodigoConsulta { get; set; } = string.Empty; // Ej: "FACTURACION_VENTAS"
        public string? Descripcion { get; set; }
        public string NombreConexion { get; set; } = string.Empty;
        public string IdProveedor { get; set; } = string.Empty;
        public string PlantillaParametrosXml { get; set; } = string.Empty; // Ej: "<F200_NIT>{{Nit}}</F200_NIT>"
        public bool Estado { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
