using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.Auth;
using Nexus.Application.Helpers;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities;

namespace Nexus.Application.Services;

/// <summary>
/// Implementación del servicio de autenticación: registro y login de usuarios.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IAplicacionRepository _aplicacionRepository;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IAplicacionRepository aplicacionRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _aplicacionRepository = aplicacionRepository;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await _userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken))
        {
            throw new BadRequestException("Ya existe un usuario registrado con ese correo electrónico.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = "User",
            IsActive = true
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            // Mensaje deliberadamente genérico: no revela si el correo existe o no.
            throw new UnauthorizedAppException("Correo electrónico o contraseña incorrectos.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAppException("El usuario se encuentra inactivo. Contacte al administrador.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.SaveChangesAsync(cancellationToken);

        return BuildAuthResponse(user);
    }

    private AuthResponseDto BuildAuthResponse(User user)
    {
        var (token, expiresAt) = _jwtService.GenerarToken(user);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            }
        };
    }

    public async Task<AutenticarAplicacionResponseDto> AutenticarAplicacionAsync(AutenticarAplicacionRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.CodigoAplicacion) ||
        string.IsNullOrWhiteSpace(request.ClientId) ||
        string.IsNullOrWhiteSpace(request.ClientSecret))
        {
            throw new ArgumentException("El código de aplicación, ClientId y ClientSecret son obligatorios.");
        }

        // 1. Buscar la aplicación por CodigoAplicacion en PostgreSQL
        var app = await _aplicacionRepository.GetByCodigoAsync(request.CodigoAplicacion);

        // 2. Validar existencia, coincidencia exacta de ClientId y estados requeridos
        if (app == null ||
            !string.Equals(app.ClientId, request.ClientId, StringComparison.Ordinal) ||
            !app.Estado ||
            !app.OnboardingCompletado)
        {
            throw new UnauthorizedAppException("Credenciales inválidas o la aplicación no está autorizada.");
        }

        // 3. Validar el Hash del ClientSecret en tiempo constante (FixedTimeEquals)
        if (string.IsNullOrWhiteSpace(app.ClientSecretHash) ||
            !SecretGeneratorHelper.ValidarSecret(request.ClientSecret, app.ClientSecretHash))
        {
            throw new UnauthorizedAppException("Credenciales inválidas.");
        }

        // 4. Generar y retornar el token JWT
        return _jwtService.GenerarJwtTokenAplicacion(app.ClientId!, app.CodigoApp, app.Nombre);
    }
}
