using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.Common;
using Nexus.Application.DTOs.Conectores;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers;

/// <summary>Catálogo de conectores/microservicios vinculados a los ERPs.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class ConectoresController : ControllerBase
{
    private readonly IConectorService _service;
    public ConectoresController(IConectorService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ConectorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodos([FromQuery] bool soloActivos = true) =>
        Ok(await _service.ObtenerTodosAsync(soloActivos));

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ConectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var conector = await _service.ObtenerPorIdAsync(id);
        return conector is null ? NotFound() : Ok(conector);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ConectorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] CrearConectorDto dto)
    {
        var creado = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarConectorDto dto)
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
