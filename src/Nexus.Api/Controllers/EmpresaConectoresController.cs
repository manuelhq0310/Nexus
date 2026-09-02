using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.Common;
using Nexus.Application.DTOs.EmpresaConectores;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers;

/// <summary>Relación directa Empresa &lt;-&gt; Conector (relación 1 a 1: qué conector usa cada empresa).</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class EmpresaConectoresController : ControllerBase
{
    private readonly IEmpresaConectorService _service;
    public EmpresaConectoresController(IEmpresaConectorService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmpresaConectorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodas([FromQuery] bool soloActivas = true) =>
        Ok(await _service.ObtenerTodasAsync(soloActivas));

    /// <summary>Devuelve el conector asociado a la empresa, o 404 si no tiene ninguno (relación 1 a 1).</summary>
    [HttpGet("empresa/{empresaId:long}")]
    [ProducesResponseType(typeof(EmpresaConectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorEmpresa(long empresaId)
    {
        var entity = await _service.ObtenerPorEmpresaAsync(empresaId);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(EmpresaConectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var entity = await _service.ObtenerPorIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmpresaConectorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Crear([FromBody] AsignarEmpresaConectorDto dto)
    {
        var creado = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:long}/conector/{nuevoConectorId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CambiarConector(long id, long nuevoConectorId)
    {
        var actualizado = await _service.CambiarConectorAsync(id, nuevoConectorId);
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
