using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.UnoEConsultaConfig;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;
using System.Text.Json;

namespace Nexus.Api.Controllers
{
    [ApiController]
    [Route("api/unoe/consultas")]
    public class UnoEConsultasController : ControllerBase
    {
        private readonly IUnoEConsultaService _consultaService;
        private readonly ILogger<UnoEConsultasController> _logger;

        public UnoEConsultasController(
            IUnoEConsultaService consultaService,
            ILogger<UnoEConsultasController> logger)
        {
            _consultaService = consultaService;
            _logger = logger;
        }

        ///// <summary>
        ///// Endpoint expuesto a clientes externos (ej. Tickelia) para ejecutar consultas dinámicas en UnoE.
        ///// </summary>
        //[HttpPost("ejecutar")]
        //[ProducesResponseType(typeof(JsonDocument), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> EjecutarConsulta([FromBody] EjecutarConsultaRequestDto request)
        //{
        //    try
        //    {
        //        var resultadoJson = await _consultaService.EjecutarConsultaAsync(request);
        //        return Ok(resultadoJson);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        _logger.LogWarning(ex.Message);
        //        return NotFound(new { mensaje = ex.Message });
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        _logger.LogWarning(ex.Message);
        //        return Unauthorized(new { mensaje = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error procesando consulta genérica en UnoE.");
        //        return StatusCode(500, new { mensaje = "Error interno procesando la consulta.", detalle = ex.Message });
        //    }
        //}

        /// <summary>
        /// Obtiene todas las configuraciones de consultas activas registradas en Nexus.
        /// </summary>
        [HttpGet("configuraciones")]
        public async Task<IActionResult> GetConfiguraciones()
        {
            var result = await _consultaService.ObtenerConsultasActivasAsync();
            return Ok(result);
        }

        /// <summary>
        /// Registra una nueva plantilla de consulta en Nexus.
        /// </summary>
        [HttpPost("configuraciones")]
        public async Task<IActionResult> CrearConfiguracion([FromBody] UnoEConsultaConfig config)
        {
            var result = await _consultaService.RegistrarConsultaConfigAsync(config);
            return CreatedAtAction(nameof(GetConfiguraciones), new { id = result.Id }, result);
        }

        /// <summary>
        /// Obtiene una configuración de consulta por su código.
        /// </summary>
        [HttpGet("configuraciones/{codigoConsulta}")]
        public async Task<IActionResult> GetPorCodigo(string codigoConsulta)
        {
            var config = await _consultaService.ObtenerPorCodigoAsync(codigoConsulta);
            if (config == null) return NotFound(new { mensaje = $"Consulta '{codigoConsulta}' no encontrada." });
            return Ok(config);
        }

        /// <summary>
        /// Actualiza una configuración de consulta existente.
        /// </summary>
        [HttpPut("configuraciones")]
        public async Task<IActionResult> ActualizarConfiguracion([FromBody] UnoEConsultaConfig config)
        {
            try
            {
                var actualizado = await _consultaService.ActualizarConsultaConfigAsync(config);
                return Ok(new { exito = actualizado });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Activa o desacativa una consulta de UnoE.
        /// </summary>
        [HttpPatch("configuraciones/{codigoConsulta}/cambiar-estado")]
        public async Task<IActionResult> CambiarEstado(string codigoConsulta, [FromQuery] bool estado)
        {
            try
            {
                var resultado = await _consultaService.CambiarEstadoAsync(codigoConsulta, estado);
                return Ok(new { exito = resultado, mensaje = $"Consulta '{codigoConsulta}' ahora está en estado {(estado ? "Activo" : "Inactivo")}." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }
    }
}
