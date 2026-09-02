using System.Security.Cryptography;
using Nexus.Application.Interfaces.Services;

namespace Nexus.Infrastructure.Security;

/// <summary>
/// Implementación de hashing de contraseñas usando PBKDF2 (Rfc2898DeriveBytes) con
/// salt aleatorio por usuario. No requiere dependencias externas y es el estándar
/// recomendado por Microsoft para almacenamiento seguro de contraseñas.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;       // 128 bits
    private const int KeySize = 32;        // 256 bits
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        // Formato autocontenido: iteraciones.salt.hash (todo en Base64 excepto el número de iteraciones)
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string password, string hash)
    {
        var parts = hash.Split('.', 3);
        if (parts.Length != 3)
        {
            return false;
        }

        var iterations = int.Parse(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var expectedKey = Convert.FromBase64String(parts[2]);

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expectedKey.Length);

        // Comparación en tiempo constante para evitar timing attacks.
        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }
}
