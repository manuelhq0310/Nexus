using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.Common;
using Nexus.Application.DTOs.Integraciones;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers;

/// <summary>Catálogo maestro de acciones/procesos de negocio estandarizados.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class IntegracionesController : ControllerBase
{
    private readonly IIntegracionService _service;
    public IntegracionesController(IIntegracionService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IntegracionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodas([FromQuery] bool soloActivas = true) =>
        Ok(await _service.ObtenerTodasAsync(soloActivas));

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(IntegracionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var integracion = await _service.ObtenerPorIdAsync(id);
        return integracion is null ? NotFound() : Ok(integracion);
    }

    [HttpGet("codigo/{codigoAccion}")]
    [ProducesResponseType(typeof(IntegracionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorCodigoAccion(string codigoAccion)
    {
        var integracion = await _service.ObtenerPorCodigoAccionAsync(codigoAccion);
        return integracion is null ? NotFound() : Ok(integracion);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IntegracionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] CrearIntegracionDto dto)
    {
        var creado = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarIntegracionDto dto)
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
