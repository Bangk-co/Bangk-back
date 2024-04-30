using System.IO;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

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

        public static string EncryptAes(string plainText, string key, string iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static int GenerateRandomNumber()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        public static bool IsValidEmail(string email)
        {
            // Define a regular expression for a basic email validation
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";

            // Create a Regex object and match the email against the pattern
            Regex regex = new Regex(pattern);
            Match match = regex.Match(email);

            // Return true if the email matches the pattern, otherwise false
            return match.Success;
        }


        public static bool IsValidPassword(string password)
        {
            string passwordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[#?!@$%^&*-]).{8,}$";
            var regex = new Regex(passwordPattern);
            return regex.IsMatch(password);
        }
    }
}
