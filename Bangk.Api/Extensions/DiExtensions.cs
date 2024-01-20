using Bangk.Common.Dto;
using Bangk.Common.Validators;
using Bangk.Domain.Contract;
using Bangk.Domain.Contract.Costumer;
using Bangk.Domain.Contract.Files;
using Bangk.Domain.Contract.GroupSettings;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Domain.Contract.Login;
using Bangk.Domain.Contract.Settings;
using Bangk.Domain.Contract.Users;
using Bangk.Domain.Implementation;
using Bangk.Domain.Implementation.Client;
using Bangk.Domain.Implementation.Files;
using Bangk.Domain.Implementation.GroupSettings;
using Bangk.Domain.Implementation.LoanApplication;
using Bangk.Domain.Implementation.Login;
using Bangk.Domain.Implementation.Settings;
using Bangk.Domain.Implementation.Users;
using Bangk.Repositories.Repositories;
using FluentValidation;
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
            services.AddScoped<IGroupSettingRepository, GroupSettingRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
            services.AddScoped<ILoanApplicationDocumentRepository, LoanApplicationDocumentRepository>();

            // Domains
            services.AddTransient<ILogDomain, LogDomain>();
            services.AddTransient<IUserLoginService, UserLoginService>();
            services.AddTransient<ICreateTokenService, CreateTokenService>();
            services.AddTransient<IResetPasswordService, ResetPasswordService>();
            services.AddTransient<IGetUserByHashEmailResetService, GetUserByHashEmailResetService>();
            services.AddTransient<IGetUserByEmailService, GetUserByEmailService>();
            services.AddTransient<ISendEmailResetPassword, SendEmailResetPassword>();
            services.AddTransient<IUploadFileService, UploadFileService>();
            services.AddTransient<IGetGroupSettingListService, GetGroupSettingListService>();
            services.AddTransient<IGetSettingListService, GetSettingListService>();
            services.AddTransient<ICostumerService, GetClientService>();
            services.AddTransient<ILoanApplicationServices, GetLoanApplicationServices>();
            // Add AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //Azure Blob Storage
        }
    }
}