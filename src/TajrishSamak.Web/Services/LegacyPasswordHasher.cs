using System.Security.Cryptography;

namespace TajrishSamak.Web.Services
{
    /// <summary>
    /// Verifies and creates ASP.NET Identity 2.x password hashes (format V2)
    /// so existing users from the legacy MVC database can log in unchanged.
    /// Format: base64( [0]=0x00 version | 16-byte salt | PBKDF2-HMAC-SHA1(password, salt, 1000, 32-byte key) )
    /// ASP.NET Core Identity's V3 format (0x01) is also verified for safety.
    /// </summary>
    public static class LegacyPasswordHasher
    {
        private const byte V2Version = 0x00;
        private const byte V3Version = 0x01;
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int V2Iterations = 1000;

        public static bool Verify(string hashed, string password)
        {
            if (string.IsNullOrWhiteSpace(hashed) || password == null)
            {
                return false;
            }

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(hashed);
            }
            catch (FormatException)
            {
                return false;
            }

            if (bytes.Length == 0)
            {
                return false;
            }

            if (bytes[0] == V2Version)
            {
                if (bytes.Length != 1 + SaltSize + KeySize)
                {
                    return false;
                }

                var salt = bytes.AsSpan(1, SaltSize);
                var expected = bytes.AsSpan(1 + SaltSize);

                using var derive = new Rfc2898DeriveBytes(password, salt.ToArray(), V2Iterations, HashAlgorithmName.SHA1);
                var actual = derive.GetBytes(expected.Length);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }

            if (bytes[0] == V3Version && bytes.Length > 13)
            {
                // ASP.NET Core V3 header: [1]=prf, [2..6]=iterations, [6..10]=saltLen, [10..14]=keyLen
                var prf = bytes[1];
                var iterations = BitConverter.ToInt32(bytes, 2);
                var saltLength = BitConverter.ToInt32(bytes, 6);
                var keyLength = BitConverter.ToInt32(bytes, 10);

                if (iterations < 1 || saltLength <= 0 || keyLength <= 0 || bytes.Length != 14 + saltLength + keyLength)
                {
                    return false;
                }

                var algorithm = prf == 0 ? HashAlgorithmName.SHA1 : HashAlgorithmName.SHA256;
                var salt = bytes.AsSpan(14, saltLength);
                var expected = bytes.AsSpan(14 + saltLength);

                using var derive = new Rfc2898DeriveBytes(password, salt.ToArray(), iterations, algorithm);
                var actual = derive.GetBytes(expected.Length);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }

            return false;
        }

        /// <summary>Creates a V2-format hash (identical to legacy Identity 2 storage).</summary>
        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            using var derive = new Rfc2898DeriveBytes(password, salt, V2Iterations, HashAlgorithmName.SHA1);
            var subkey = derive.GetBytes(KeySize);

            var output = new byte[1 + SaltSize + KeySize];
            output[0] = V2Version;
            salt.CopyTo(output, 1);
            subkey.CopyTo(output, 1 + SaltSize);
            return Convert.ToBase64String(output);
        }
    }
}
