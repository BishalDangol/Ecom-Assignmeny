using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Saja.Utilities
{
    /// <summary>
    /// Utility class for security operations like hashing.
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// Hashes a string using SHA256.
        /// </summary>
        /// <param name="rawData">Plain text data</param>
        /// <returns>Hashed string</returns>
        public static string HashPassword(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Verifies a password against a hash.
        /// </summary>
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            string hashOfInput = HashPassword(inputPassword);
            return String.Compare(hashOfInput, storedHash, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Generates a random token for persistent login.
        /// </summary>
        public static string GenerateToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
