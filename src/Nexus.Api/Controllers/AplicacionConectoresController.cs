using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.AplicacionConectores;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers;

/// <summary>Qué Conectores puede usar cada Aplicación, con sus credenciales de autenticación.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class AplicacionConectoresController : ControllerBase
{
    private readonly IAplicacionConectorService _service;
    public AplicacionConectoresController(IAplicacionConectorService service) => _service = service;

    [HttpGet("aplicacion/{aplicacionId:long}")]
    [ProducesResponseType(typeof(IEnumerable<AplicacionConectorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerPorAplicacion(long aplicacionId) =>
        Ok(await _service.ObtenerPorAplicacionAsync(aplicacionId));

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AplicacionConectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var entity = await _service.ObtenerPorIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AplicacionConectorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Crear([FromBody] CrearAplicacionConectorDto dto)
    {
        var creado = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarAplicacionConectorDto dto)
    {
        var actualizado = await _service.ActualizarAsync(id, dto);
        return actualizado ? NoContent() : NotFound();
    }

    /// <summary>
    /// A diferencia de los demás endpoints "estado" de la API, este recibe un booleano crudo
    /// en el body (no { activo }), tal como está definido en el contrato original.
    /// </summary>
    [HttpPatch("{id:long}/estado")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] bool activo)
    {
        var actualizado = await _service.CambiarEstadoAsync(id, activo);
        return actualizado ? NoContent() : NotFound();
    }
}
