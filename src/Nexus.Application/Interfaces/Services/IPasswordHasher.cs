namespace Nexus.Application.Interfaces.Services;

/// <summary>
/// Servicio encargado del hashing y verificación segura de contraseñas.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
