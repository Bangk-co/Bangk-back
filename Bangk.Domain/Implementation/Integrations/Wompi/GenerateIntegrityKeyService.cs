using Bangk.Domain.Contract.Integrations.Wompi;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations.Wompi
{
    public class GenerateIntegrityKeyService : IGenerateIntegrityKeyService
    {
        private readonly IConfiguration _configuration;
        public GenerateIntegrityKeyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> Execute(long amount, string paymentReference)
        {
            var integrityKey = _configuration.GetSection("WompiKeys:IntegrityKey").Value;
            var secretKey = _configuration.GetSection("WompiKeys:SecretKey").Value;
            var resultIntegrity = paymentReference + amount + "COP" + integrityKey;
            return GenerateHmacSha256Signature(resultIntegrity, integrityKey);
        }

        static string GenerateHmacSha256Signature(string data, string key)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower(); // Convertir a hexadecimal
            }
        }
    }
}
