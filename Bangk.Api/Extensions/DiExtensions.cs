using FluentValidation;
using Bangk.Common.Dto;
using Bangk.Common.Validators;
using Bangk.Domain.Contract;
using Bangk.Domain.Contract.Login;
using Bangk.Domain.Contract.Users;
using Bangk.Domain.Implementation;
using Bangk.Domain.Implementation.Login;
using Bangk.Domain.Implementation.Users;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bangk.Api.Extensions
{
    public static class DiExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // Fluent Validations
            services.AddSingleton<IValidator<LoginDto>, LoginValidator>();
            services.AddSingleton<IValidator<ResetPasswordDto>, ResetPasswordValidator>(); 
            // Repositories
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            // Domains
            services.AddTransient<ILogDomain, LogDomain>();
            services.AddTransient<IUserLoginService, UserLoginService>();
            services.AddTransient<ICreateTokenService, CreateTokenService>();
            services.AddTransient<IResetPasswordService, ResetPasswordService>();
            services.AddTransient<IGetUserByHashEmailResetService, GetUserByHashEmailResetService>();
            services.AddTransient<IGetUserByEmailService, GetUserByEmailService>();
            services.AddTransient<ISendEmailResetPassword, SendEmailResetPassword>(); 

            // Add AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
           
        }
    }
}