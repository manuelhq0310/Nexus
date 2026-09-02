using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.Common;
using Nexus.Application.DTOs.IntegracionConectores;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers;

/// <summary>Qué conector resuelve cada integración, con su ruta de endpoint y cola de RabbitMQ.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class IntegracionConectoresController : ControllerBase
{
    private readonly IIntegracionConectorService _service;
    public IntegracionConectoresController(IIntegracionConectorService service) => _service = service;

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(IntegracionConectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var entity = await _service.ObtenerPorIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpGet("integracion/{integracionId:long}")]
    [ProducesResponseType(typeof(IEnumerable<IntegracionConectorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerPorIntegracion(long integracionId) =>
        Ok(await _service.ObtenerPorIntegracionAsync(integracionId));

    [HttpGet("conector/{conectorId:long}")]
    [ProducesResponseType(typeof(IEnumerable<IntegracionConectorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerPorConector(long conectorId) =>
        Ok(await _service.ObtenerPorConectorAsync(conectorId));

    [HttpPost]
    [ProducesResponseType(typeof(IntegracionConectorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Crear([FromBody] CrearIntegracionConectorDto dto)
    {
        var creado = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarIntegracionConectorDto dto)
    {
        var actualizado = await _service.ActualizarAsync(id, dto);
        return actualizado ? NoContent() : NotFound();
    }

    [HttpPatch("{id:long}/estado")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoDto dto)
    {
        var actualizado = await _service.CambiarEstadoAsync(id, dto.Activo);
        return actualizado ? NoContent() : NotFound();
    }
}
