using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.IntegracionRouter;
using Nexus.Application.Interfaces.Services.V2;

namespace Nexus.Api.Controllers.V2
{
	[ApiController]
	[Route("api/v2/[controller]")]
	public class IntegracionRouterController : ControllerBase
    {
		private readonly IIntegracionRouterService _payanaService;

		public IntegracionRouterController(IIntegracionRouterService payanaService)
		{
			_payanaService = payanaService;
		}

		[HttpGet("ejecutar-consulta")]
		[ProducesResponseType(typeof(EjecutarConsultaResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> EjecutarConsulta(
		[FromQuery(Name = "idCompania")] int idCompania,
		[FromQuery(Name = "descripcion")] string descripcion,
		[FromQuery(Name = "codigoAplicacion")] string codigoAplicacion,
		[FromQuery(Name = "paginacion")] string? paginacionStr,
		[FromQuery(Name = "parametros")] string? parametrosStr,
		[FromHeader(Name = "connikey")] string connikey,
		[FromHeader(Name = "conniToken")] string conniToken)
		{
			var resultado = await _payanaService.EjecutarConsultaAsync(idCompania, descripcion, codigoAplicacion, paginacionStr, parametrosStr, connikey, conniToken);

			if (resultado.Codigo != 0)
			{
				return BadRequest(resultado);
			}

			return Ok(resultado);
		}

		[HttpPost("ejecutar-integracion")]
		[ProducesResponseType(typeof(EjecutarIntegracionResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> EjecutarIntegracion([FromQuery] int idCompania,
														    [FromQuery] string? idDocumento,
														    [FromQuery] string? nombreDocumento,
														    [FromQuery] string? codigoAplicacion,
														    [FromHeader] string connikey,
														    [FromHeader] string conniToken,
															[FromBody] dynamic request)
		{
			var resultado = await _payanaService.EjecutarIntegracionAsync(idCompania, idDocumento, nombreDocumento, codigoAplicacion, request, connikey, conniToken);

			if (resultado.Codigo != 0)
			{
				return BadRequest(resultado);
			}

			return Ok(resultado);
		}
	}
}
