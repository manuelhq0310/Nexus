using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexus.Application.DTOs.IntegracionRouter;
using Nexus.Application.DTOs.UnoEConsultaConfig;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;
using System.Text.Json;

namespace Nexus.Application.Services
{
    public class UnoEConsultaService : IUnoEConsultaService
    {
        private readonly IUnoEConsultaConfigRepository _consultaConfigRepo;
        private readonly IAplicacionConectorRepository _aplicacionConectorRepo;
        private readonly IAplicacionRepository _aplicacionRepo;
        //private readonly IUnoEConectorClient _unoEConectorClient;
        private readonly ILogger<UnoEConsultaService> _logger;

        public UnoEConsultaService(
            IUnoEConsultaConfigRepository consultaConfigRepo,
            IAplicacionConectorRepository aplicacionConectorRepo,
            IAplicacionRepository aplicacionRepo,
            //IUnoEConectorClient unoEConectorClient,
            ILogger<UnoEConsultaService> logger)
        {
            _consultaConfigRepo = consultaConfigRepo;
            _aplicacionConectorRepo = aplicacionConectorRepo;
            _aplicacionRepo = aplicacionRepo;
            //_unoEConectorClient = unoEConectorClient;
            _logger = logger;
        }

        //public async Task<JsonDocument> EjecutarConsultaAsync(EjecutarConsultaRequestDto request)
        //{
        //    var configConsulta = await _consultaConfigRepo.GetByCodigoAsync(request.CodigoConsulta)
        //        ?? throw new KeyNotFoundException($"No existe configuración activa para la consulta: '{request.CodigoConsulta}'.");

        //    var app = await _aplicacionRepo.GetByCodigoAsync(request.CodigoApp)
        //        ?? throw new KeyNotFoundException($"Aplicación '{request.CodigoApp}' no encontrada.");

        //    var appConector = await _aplicacionConectorRepo.GetByAplicacionIdAsync(app.Id)
        //        ?? throw new UnauthorizedAccessException($"No hay credenciales ERP configuradas en IntgAplicacionConector para App '{request.CodigoApp}'.");

        //    int.TryParse(request.NumeroIdentificacionEmpresa, out int idCia);

        //    var conectorRequest = new ConsultaGeneralRequest
        //    {
        //        NombreConexion = configConsulta.NombreConexion,
        //        IdCia = idCia,
        //        IdProveedor = configConsulta.IdProveedor,
        //        IdConsulta = configConsulta.CodigoConsulta,
        //        Usuario = appConector.UsuarioErp,
        //        Clave = appConector.PasswordErp,
        //        Parametros = request.ParametrosXml
        //    };

        //    _logger.LogInformation("Invocando UnoEConector.ConsultaGeneral para Consulta: {Codigo}, App: {App}", request.CodigoConsulta, request.CodigoApp);

        //    return await _unoEConectorClient.ConsultaGeneral(conectorRequest);
        //}

        public async Task<object> PrepararConsultaGenerica(EjecutarIntegracionRequest request, IntgAplicacionConector aplicacionConector)
        {
            var payload = JsonConvert.DeserializeObject<dynamic>(request.Payload.ToString());
            var configConsulta = await _consultaConfigRepo.GetByCodigoAsync(payload.IdConsulta)
                ?? throw new KeyNotFoundException($"No existe configuración activa para la consulta: '{request.CodigoIntegracion}'.");

            var conectorRequest = new ConsultaGeneralRequest
            {
                NombreConexion = configConsulta.NombreConexion,
                IdCia = request.CodigoEmpresa,
                IdProveedor = configConsulta.IdProveedor,
                IdConsulta = configConsulta.CodigoConsulta,
                Usuario = aplicacionConector.UsuarioErp,
                Clave = aplicacionConector.PasswordErp,
                Parametros = payload.Parametros.ToString()
            };

            return conectorRequest;
        }

        public async Task<UnoEConsultaConfig?> ObtenerPorCodigoAsync(string codigoConsulta)
        {
            return await _consultaConfigRepo.GetByCodigoAsync(codigoConsulta);
        }

        public async Task<IEnumerable<UnoEConsultaConfig>> ObtenerConsultasActivasAsync()
        {
            return await _consultaConfigRepo.GetAllActivasAsync();
        }

        public async Task<UnoEConsultaConfig> RegistrarConsultaConfigAsync(UnoEConsultaConfig config)
        {
            _logger.LogInformation("Registrando nueva consulta UnoE: {Codigo}", config.CodigoConsulta);
            return await _consultaConfigRepo.AddAsync(config);
        }

        public async Task<bool> ActualizarConsultaConfigAsync(UnoEConsultaConfig config)
        {
            var existe = await _consultaConfigRepo.GetByCodigoAsync(config.CodigoConsulta);
            if (existe == null)
            {
                throw new KeyNotFoundException($"No se puede actualizar. No existe la consulta '{config.CodigoConsulta}'.");
            }

            _logger.LogInformation("Actualizando configuración de consulta UnoE: {Codigo}", config.CodigoConsulta);
            return await _consultaConfigRepo.UpdateAsync(config);
        }

        public async Task<bool> CambiarEstadoAsync(string codigoConsulta, bool nuevoEstado)
        {
            var consulta = await _consultaConfigRepo.GetByCodigoAsync(codigoConsulta);
            if (consulta == null)
            {
                throw new KeyNotFoundException($"No se encontró la consulta '{codigoConsulta}' para modificar su estado.");
            }

            consulta.Estado = nuevoEstado;
            _logger.LogInformation("Cambiando estado de consulta UnoE [{Codigo}] a: {Estado}", codigoConsulta, nuevoEstado);

            return await _consultaConfigRepo.UpdateAsync(consulta);
        }
    }
}
