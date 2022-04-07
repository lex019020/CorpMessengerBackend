using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CorpMessengerBackend.Services
{
    public class CryptographyService
    {
        private const string AllowableCharacters =
            "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";

        public static string GenerateNewToken()
        {
            var length = new Random(Environment.TickCount).Next(96, 128);

            var bytes = new byte[length];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(bytes);
            }

            return new string(bytes.Select(x => AllowableCharacters[x % AllowableCharacters.Length]).ToArray());
        }

        public static string HashPassword(string password, byte[]? salt = null)
        {
            if (salt == null || salt.Length < 8)
            {
                // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
                salt = new byte[128 / 8];

                using (var rngCsp = new RNGCryptoServiceProvider())
                {
                    rngCsp.GetNonZeroBytes(salt);
                } 
            }

            // derive a 256-bit subkey (use HMACSHA256 with 2,500 iterations)
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 250000,
                numBytesRequested: 256 / 8));

            var saltString = Convert.ToBase64String(salt);

            return saltString + ":" + hashed;
        }

        public static bool CheckPassword(string password, string secret)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (string.IsNullOrEmpty(secret))
                return false;

            var secretData = secret.Split(':');

            if (secretData.Length != 2)
            {
                return false;
                // todo log
            }

            var salt = Convert.FromBase64String(secretData[0]);

            var hashed = HashPassword(password, salt);

            return hashed == secret;
        }
    }
}
