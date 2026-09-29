using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nexus.Application.DTOs.Auth;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities;

namespace Nexus.Infrastructure.Security;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public (string Token, DateTime ExpiresAt) GenerarToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };
        string tokenString = GenerarTokenString(expiresAt, claims);
        return (tokenString, expiresAt);
    }

    private string GenerarTokenString(DateTime expiresAt, Claim[] claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }

    public AutenticarAplicacionResponseDto GenerarJwtTokenAplicacion(string clientId, string codigoApp, string nombreApp)
    {
        var secretKey = Encoding.UTF8.GetBytes(_settings.Secret);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, clientId),
            new Claim("client_id", clientId),
            new Claim("codigo_app", codigoApp),
            new Claim("nombre_app", nombreApp),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var fechaExpiracion = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);
        string tokenString = GenerarTokenString(fechaExpiracion, claims);

        return new AutenticarAplicacionResponseDto
        {
            AccessToken = tokenString,
            TokenType = "Bearer",
            ExpiresAt = fechaExpiracion
        };
    }
}
