using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.IntegracionRouter;
using Nexus.Application.DTOs.UnoEConsultaConfig;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;
using System.Text;
using System.Xml.Linq;

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

        public async Task<object> PrepararConsultaGenerica(EjecutarIntegracionRequest request, IntgAplicacionConector aplicacionConector)
        {
            var payloadJson = JObject.Parse(request.Payload.ToString());

            string nombreConsulta = payloadJson["idConsulta"]?.ToString()
                                    ?? payloadJson["IdConsulta"]?.ToString()
                                    ?? throw new BadRequestException("No se especificó un código de consulta válido en el request.");

            var configConsulta = await _consultaConfigRepo.GetByCodigoAsync(nombreConsulta)
                ?? throw new NotFoundException($"No existe configuración activa para la consulta: '{nombreConsulta}'.");

            string xmlParametros = ConstruirXmlParametros(payloadJson["parametros"] ?? payloadJson["Parametros"]);

            var conectorRequest = new ConsultaGeneralRequest
            {
                NombreConexion = configConsulta.NombreConexion,
                IdCia = request.CodigoEmpresa,
                IdProveedor = configConsulta.IdProveedor,
                IdConsulta = configConsulta.CodigoConsulta,
                Usuario = aplicacionConector.UsuarioErp,
                Clave = aplicacionConector.PasswordErp,
                Parametros = xmlParametros
            };

            return conectorRequest;
        }

        private string ConstruirXmlParametros(JToken parametrosToken)
        {
            if (parametrosToken == null || !parametrosToken.HasValues)
            {
                return string.Empty;
            }

            var xmlBuilder = new StringBuilder();

            if (parametrosToken is JObject parametrosObject)
            {
                foreach (var propiedad in parametrosObject.Properties())
                {
                    var nombreEtiqueta = propiedad.Name;
                    var valor = propiedad.Value?.ToString() ?? string.Empty;

                    // Escapa caracteres especiales automáticamente
                    var elemento = new XElement(nombreEtiqueta, valor);
                    xmlBuilder.Append(elemento.ToString(SaveOptions.DisableFormatting));
                }
            }

            return xmlBuilder.ToString();
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
                throw new NotFoundException($"No se puede actualizar. No existe la consulta '{config.CodigoConsulta}'.");
            }

            _logger.LogInformation("Actualizando configuración de consulta UnoE: {Codigo}", config.CodigoConsulta);
            return await _consultaConfigRepo.UpdateAsync(config);
        }

        public async Task<bool> CambiarEstadoAsync(string codigoConsulta, bool nuevoEstado)
        {
            var consulta = await _consultaConfigRepo.GetByCodigoAsync(codigoConsulta);
            if (consulta == null)
            {
                throw new NotFoundException($"No se encontró la consulta '{codigoConsulta}' para modificar su estado.");
            }

            consulta.Estado = nuevoEstado;
            _logger.LogInformation("Cambiando estado de consulta UnoE [{Codigo}] a: {Estado}", codigoConsulta, nuevoEstado);

            return await _consultaConfigRepo.UpdateAsync(consulta);
        }
    }
}
