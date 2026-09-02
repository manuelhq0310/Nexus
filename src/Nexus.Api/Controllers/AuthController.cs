using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.Application.DTOs.Auth;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Api.Controllers;

/// <summary>
/// Endpoints de autenticación y registro de usuarios.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <response code="201">Usuario creado correctamente. Devuelve el token JWT.</response>
    /// <response code="400">Datos inválidos o correo ya registrado.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Register), new { id = result.User.Id }, result);
    }

    /// <summary>
    /// Autentica un usuario y devuelve un token JWT para consumir los servicios privados.
    /// </summary>
    /// <response code="200">Autenticación exitosa.</response>
    /// <response code="401">Credenciales inválidas.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Devuelve la información del usuario autenticado a partir del token JWT.
    /// Sirve como ejemplo de endpoint privado protegido con [Authorize].
    /// </summary>
    /// <response code="200">Información del usuario autenticado.</response>
    /// <response code="401">Token ausente o inválido.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}
