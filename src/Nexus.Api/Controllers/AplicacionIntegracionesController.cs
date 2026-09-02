using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.AplicacionIntegraciones;
using Nexus.Application.DTOs.Common;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers;

/// <summary>Qué Integraciones puede ejecutar cada Aplicación.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class AplicacionIntegracionesController : ControllerBase
{
    private readonly IAplicacionIntegracionService _service;
    public AplicacionIntegracionesController(IAplicacionIntegracionService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AplicacionIntegracionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodas([FromQuery] bool soloActivas = true) =>
        Ok(await _service.ObtenerTodasAsync(soloActivas));

    [HttpGet("aplicacion/{aplicacionId:long}")]
    [ProducesResponseType(typeof(IEnumerable<AplicacionIntegracionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerPorAplicacion(long aplicacionId, [FromQuery] bool soloActivas = true) =>
        Ok(await _service.ObtenerPorAplicacionAsync(aplicacionId, soloActivas));

    [HttpGet("integracion/{integracionId:long}")]
    [ProducesResponseType(typeof(IEnumerable<AplicacionIntegracionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerPorIntegracion(long integracionId, [FromQuery] bool soloActivas = true) =>
        Ok(await _service.ObtenerPorIntegracionAsync(integracionId, soloActivas));

    [HttpGet("{aplicacionId:long}/{integracionId:long}")]
    [ProducesResponseType(typeof(AplicacionIntegracionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorClaveCompuesta(long aplicacionId, long integracionId)
    {
        var entity = await _service.ObtenerPorClaveCompuestaAsync(aplicacionId, integracionId);
        return entity is null ? NotFound() : Ok(entity);
    }

    /// <summary>
    /// Crea la relación Aplicación-Integración. Devuelve 201 sin cuerpo: esta relación no tiene
    /// un identificador propio en el contrato de la API (se identifica por su clave compuesta).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Crear([FromBody] CrearAplicacionIntegracionDto dto)
    {
        await _service.CrearAsync(dto);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPatch("{aplicacionId:long}/{integracionId:long}/estado")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CambiarEstado(long aplicacionId, long integracionId, [FromBody] CambiarEstadoDto dto)
    {
        var actualizado = await _service.CambiarEstadoAsync(aplicacionId, integracionId, dto.Activo);
        return actualizado ? NoContent() : NotFound();
    }
}
