using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Bangk.Api.Extensions
{
    public static class JwtAuthenticationExtension
    {
        public static void AddJwtAuthentication(this IServiceCollection services, string jwtToken)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtToken)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }
    }
}
