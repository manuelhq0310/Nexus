using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.IntegracionRouter;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class IntegracionRouterController : ControllerBase
    {
        private readonly IIntegracionRouterService _routerService;

        public IntegracionRouterController(IIntegracionRouterService routerService)
        {
            _routerService = routerService;
        }

        [HttpPost("ejecutar")]
        [ProducesResponseType(typeof(EjecutarIntegracionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Ejecutar([FromBody] EjecutarIntegracionRequest request)
        {
            var resultado = await _routerService.ProcesarIntegracionAsync(request);

            if (!resultado.Exitoso)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }
    }
}
