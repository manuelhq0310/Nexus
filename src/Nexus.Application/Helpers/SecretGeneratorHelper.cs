using System.Security.Cryptography;
using System.Text;

namespace Nexus.Application.Helpers
{
    public static class SecretGeneratorHelper
    {
        /// <summary>
        /// Genera un ClientId con formato único basado en el código de la aplicación.
        /// Ejemplo: "app_payana_8f9a2b3c"
        /// </summary>
        public static string GenerarClientId(string codigoApp)
        {
            var randomBytes = new byte[4];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var suffix = Convert.ToHexString(randomBytes).ToLower();

            var cleanCode = codigoApp.Trim().ToLower().Replace(" ", "_");
            return $"app_{cleanCode}_{suffix}";
        }

        /// <summary>
        /// Genera un ClientSecret aleatorio de alta entropía (256 bits / 32 bytes) codificado en hexadecimal con prefijo.
        /// Ejemplo: "nx_sec_9f8a2b3c4d5e6f7a8b9c0d1e2f3a4b5c6d7e8f9a0b1c2d3e4f5a6b7c8d9e0f1a"
        /// </summary>
        public static string GenerarClientSecret()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return $"nx_sec_{Convert.ToHexString(randomBytes).ToLower()}";
        }

        /// <summary>
        /// Genera un token efímero de un solo uso para el proceso de onboarding.
        /// Ejemplo: "onb_7a8b9c0d1e2f3a4b5c6d7e8f9a0b1c2d"
        /// </summary>
        public static string GenerarOnboardingToken()
        {
            var randomBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return $"onb_{Convert.ToHexString(randomBytes).ToLower()}";
        }

        /// <summary>
        /// Calcula el Hash SHA256 de un ClientSecret o Token para su almacenamiento seguro en la base de datos.
        /// </summary>
        public static string HashSecret(string secret)
        {
            if (string.IsNullOrWhiteSpace(secret))
                return string.Empty;

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(secret));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Compara en tiempo constante un secreto enviado en texto plano con el hash almacenado en BD 
        /// para prevenir ataques de tiempo (Timing Attacks).
        /// </summary>
        public static bool ValidarSecret(string secretEnviado, string? hashAlmacenado)
        {
            if (string.IsNullOrWhiteSpace(secretEnviado) || string.IsNullOrWhiteSpace(hashAlmacenado))
                return false;

            var hashEnviado = HashSecret(secretEnviado);

            var bytesEnviado = Encoding.UTF8.GetBytes(hashEnviado);
            var bytesAlmacenado = Encoding.UTF8.GetBytes(hashAlmacenado);

            return CryptographicOperations.FixedTimeEquals(bytesEnviado, bytesAlmacenado);
        }
    }
}
