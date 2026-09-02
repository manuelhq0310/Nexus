using Nexus.Domain.Entities;

namespace Nexus.Application.Interfaces.Services;

/// <summary>
/// Servicio encargado de la generación de tokens JWT.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Genera un token JWT firmado para el usuario indicado.
    /// </summary>
    /// <returns>El token y su fecha de expiración (UTC).</returns>
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
