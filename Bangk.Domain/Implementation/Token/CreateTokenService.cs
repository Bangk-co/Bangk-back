using Bangk.Domain.Contract.Login;
using Bangk.Repositories.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bangk.Domain.Implementation.Login
{
    public class CreateTokenService : ICreateTokenService
    {
        private readonly IConfiguration _configuration;

        public CreateTokenService(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public string Execute(User user)
        {
            var claims = new Claim[]
            {
                new(ClaimTypes.NameIdentifier, $"{user.Id}"),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.RolName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Token").Value));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(int.Parse(_configuration.GetSection("Jwt:ExpirationToken").Value)),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
