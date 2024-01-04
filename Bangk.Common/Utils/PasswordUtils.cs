using System.IO;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Bangk.Common.Utils
{
    public static class PasswordUtils
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return !computeHash.Where((t, i) => t != passwordHash[i]).Any();
        }

        public static string DecryptPassword(string encryptedPassword, string secretKey, string iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                using (var rijndaelManaged = new RijndaelManaged { Key = Encoding.UTF8.GetBytes(secretKey), IV = Encoding.UTF8.GetBytes(iv), Mode = CipherMode.CBC })
                using (var memoryStream =
                       new MemoryStream(Convert.FromBase64String(encryptedPassword)))
                using (var cryptoStream =
                       new CryptoStream(memoryStream,
                           rijndaelManaged.CreateDecryptor(Encoding.UTF8.GetBytes(secretKey), Encoding.UTF8.GetBytes(iv)),
                           CryptoStreamMode.Read))
                {
                    return new StreamReader(cryptoStream).ReadToEnd();
                }
            }
        }
    }
}
