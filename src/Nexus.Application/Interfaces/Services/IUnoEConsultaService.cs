using Nexus.Application.DTOs.IntegracionRouter;
using Nexus.Application.DTOs.UnoEConsultaConfig;
using Nexus.Domain.Entities.Integraciones;
using System.Text.Json;

namespace Nexus.Application.Interfaces.Services
{
    public interface IUnoEConsultaService
    {
        //// Método principal de orquestación
        //Task<JsonDocument> EjecutarConsultaAsync(EjecutarConsultaRequestDto request);

        // Operaciones de gestión (CRUD completo)
        Task<UnoEConsultaConfig?> ObtenerPorCodigoAsync(string codigoConsulta);
        Task<IEnumerable<UnoEConsultaConfig>> ObtenerConsultasActivasAsync();
        Task<UnoEConsultaConfig> RegistrarConsultaConfigAsync(UnoEConsultaConfig config);
        Task<bool> ActualizarConsultaConfigAsync(UnoEConsultaConfig config);
        Task<bool> CambiarEstadoAsync(string codigoConsulta, bool nuevoEstado);
        Task<object> PrepararConsultaGenerica(EjecutarIntegracionRequest request, IntgAplicacionConector aplicacionConector);
    }
}
