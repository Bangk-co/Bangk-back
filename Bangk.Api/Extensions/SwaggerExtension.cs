using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

namespace Bangk.Api.Extensions
{
    public static class SwaggerExtension
    {
        private const string DEFAULT_NAME = "Bangk API";

        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
            var assemblyVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = assemblyName ?? DEFAULT_NAME,
                    Version = $"v{assemblyVersion?.Major ?? 1}.{assemblyVersion?.Minor ?? 0}"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        @$"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. 
                        {Environment.NewLine}Example: 'Bearer XXXXXXXXXXXXXXXXXXXX'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
        }

        public static void AddCustomSwagger(this IApplicationBuilder app)
        {
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
            var assemblyVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version;

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // Collapse all tabs
                c.DocExpansion(DocExpansion.None);
                c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    $"{assemblyName ?? DEFAULT_NAME} v{assemblyVersion?.Major ?? 1}.{assemblyVersion?.Minor ?? 0}");
            });
        }
    }
}
