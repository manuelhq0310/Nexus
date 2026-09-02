using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.ConfiguracionEnrutamiento;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers;

/// <summary>
/// Matriz de enrutamiento: qué Empresa usa qué combinación Integración-Conector,
/// y resolución en tiempo de ejecución de la URL/cola final para una empresa + acción.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class ConfiguracionEnrutamientoController : ControllerBase
{
    private readonly IConfiguracionEnrutamientoService _service;
    public ConfiguracionEnrutamientoController(IConfiguracionEnrutamientoService service) => _service = service;

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(EmpresaIntegracionConectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var entity = await _service.ObtenerPorIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpGet("empresa/{empresaId:long}")]
    [ProducesResponseType(typeof(IEnumerable<EmpresaIntegracionConectorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerPorEmpresa(long empresaId) =>
        Ok(await _service.ObtenerPorEmpresaAsync(empresaId));

    [HttpGet("resolver")]
    [ProducesResponseType(typeof(RutaEnrutamientoResueltaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Resolver([FromQuery] long empresaId, [FromQuery] string codigoAccion)
    {
        var ruta = await _service.ResolverAsync(empresaId, codigoAccion);
        return ruta is null ? NotFound() : Ok(ruta);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmpresaIntegracionConectorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Crear([FromBody] CrearEmpresaIntegracionConectorDto dto)
    {
        var creado = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarEmpresaIntegracionConectorDto dto)
    {
        var actualizado = await _service.ActualizarAsync(id, dto);
        return actualizado ? NoContent() : NotFound();
    }
}
