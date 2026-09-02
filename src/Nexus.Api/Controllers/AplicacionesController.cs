using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.Aplicaciones;
using Nexus.Application.DTOs.Common;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers;

/// <summary>Aplicaciones del grupo empresarial que requieren integraciones.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class AplicacionesController : ControllerBase
{
    private readonly IAplicacionService _service;
    public AplicacionesController(IAplicacionService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AplicacionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodas([FromQuery] bool soloActivas = true) =>
        Ok(await _service.ObtenerTodasAsync(soloActivas));

    [HttpGet("codigo/{codigoApp}")]
    [ProducesResponseType(typeof(AplicacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorCodigo(string codigoApp)
    {
        var aplicacion = await _service.ObtenerPorCodigoAsync(codigoApp);
        return aplicacion is null ? NotFound() : Ok(aplicacion);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AplicacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var aplicacion = await _service.ObtenerPorIdAsync(id);
        return aplicacion is null ? NotFound() : Ok(aplicacion);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AplicacionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] CrearAplicacionDto dto)
    {
        var creada = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, creada);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarAplicacionDto dto)
    {
        var actualizada = await _service.ActualizarAsync(id, dto);
        return actualizada ? NoContent() : NotFound();
    }

    [HttpPatch("{id:long}/estado")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoDto dto)
    {
        var actualizada = await _service.CambiarEstadoAsync(id, dto.Activo);
        return actualizada ? NoContent() : NotFound();
    }
}
