using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.AplicacionEmpresas;
using Nexus.Application.DTOs.Common;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers;

/// <summary>Qué Empresas utilizan cada Aplicación.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class AplicacionEmpresasController : ControllerBase
{
    private readonly IAplicacionEmpresaService _service;
    public AplicacionEmpresasController(IAplicacionEmpresaService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AplicacionEmpresaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodas([FromQuery] bool soloActivas = true) =>
        Ok(await _service.ObtenerTodasAsync(soloActivas));

    [HttpGet("aplicacion/{aplicacionId:long}")]
    [ProducesResponseType(typeof(IEnumerable<AplicacionEmpresaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerPorAplicacion(long aplicacionId, [FromQuery] bool soloActivas = true) =>
        Ok(await _service.ObtenerPorAplicacionAsync(aplicacionId, soloActivas));

    [HttpGet("empresa/{empresaId:long}")]
    [ProducesResponseType(typeof(IEnumerable<AplicacionEmpresaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerPorEmpresa(long empresaId, [FromQuery] bool soloActivas = true) =>
        Ok(await _service.ObtenerPorEmpresaAsync(empresaId, soloActivas));

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AplicacionEmpresaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var entity = await _service.ObtenerPorIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AplicacionEmpresaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Crear([FromBody] AsignarEmpresaAAplicacionDto dto)
    {
        var creado = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
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
