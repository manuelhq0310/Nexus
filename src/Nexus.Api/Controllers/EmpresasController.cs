using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.Common;
using Nexus.Application.DTOs.Empresas;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers;

/// <summary>Compañías del grupo empresarial.</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class EmpresasController : ControllerBase
{
    private readonly IEmpresaService _service;
    public EmpresasController(IEmpresaService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmpresaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodas([FromQuery] bool soloActivas = true) =>
        Ok(await _service.ObtenerTodasAsync(soloActivas));

    [HttpGet("buscar")]
    [ProducesResponseType(typeof(EmpresaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Buscar([FromQuery] string tipoIdentificacion, [FromQuery] string numeroIdentificacion)
    {
        var empresa = await _service.ObtenerPorIdentificacionAsync(tipoIdentificacion, numeroIdentificacion);
        return empresa is null ? NotFound() : Ok(empresa);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(EmpresaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var empresa = await _service.ObtenerPorIdAsync(id);
        return empresa is null ? NotFound() : Ok(empresa);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmpresaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] CrearEmpresaDto dto)
    {
        var creada = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, creada);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarEmpresaDto dto)
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
