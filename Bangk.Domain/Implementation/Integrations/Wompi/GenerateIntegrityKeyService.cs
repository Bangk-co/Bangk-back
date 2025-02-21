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

        public async Task<string> Execute(int amount, string paymentReference)
        {
            var integrityKey = _configuration.GetSection("WompiKeys:IntegrityKey").Value;
            var secretKey = _configuration.GetSection("WompiKeys:IntegrityKey").Value;
            var resultIntegrity = $"{paymentReference}{amount}COP{integrityKey}";
            return GenerateHmacSha256Signature(resultIntegrity, secretKey);
        }

        static string GenerateHmacSha256Signature(string data, string key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
