using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.Payana;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class PayanaIntegracionController : ControllerBase
    {
		private readonly IPayanaIntegracionService _payanaService;

		public PayanaIntegracionController(IPayanaIntegracionService payanaService)
		{
			_payanaService = payanaService;
		}

		[HttpGet("ejecutarConsulta")]
		[ProducesResponseType(typeof(EjecutarConsultaResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> EjecutarConsulta(
		[FromQuery(Name = "idCompania")] int idCompania,
		[FromQuery(Name = "descripcion")] string descripcion,
		[FromQuery(Name = "paginacion")] string? paginacionStr,
		[FromQuery(Name = "parametros")] string? parametrosStr,
		[FromHeader(Name = "connikey")] string connikey,
		[FromHeader(Name = "conniToken")] string conniToken)
		{
			var resultado = await _payanaService.EjecutarConsultaAsync(idCompania, descripcion, paginacionStr, parametrosStr, connikey, conniToken);

			if (resultado.Codigo != 0)
			{
				return BadRequest(resultado);
			}

			return Ok(resultado);
		}
	}
}
