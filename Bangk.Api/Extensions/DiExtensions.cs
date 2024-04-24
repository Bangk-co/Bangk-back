using Bangk.Common.Dto;
using Bangk.Common.Validators;
using Bangk.Domain.Contract;
using Bangk.Domain.Contract.City;
using Bangk.Domain.Contract.Client;
using Bangk.Domain.Contract.Costumer;
using Bangk.Domain.Contract.Country;
using Bangk.Domain.Contract.Files;
using Bangk.Domain.Contract.GroupSettings;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Domain.Contract.LoanApplicationDocument;
using Bangk.Domain.Contract.LoanApplicationStateHistory;
using Bangk.Domain.Contract.LoanForm;
using Bangk.Domain.Contract.LoanInsurancePolicy;
using Bangk.Domain.Contract.LoanQuote;
using Bangk.Domain.Contract.Login;
using Bangk.Domain.Contract.Settings;
using Bangk.Domain.Contract.State;
using Bangk.Domain.Contract.Users;
using Bangk.Domain.Implementation;
using Bangk.Domain.Implementation.BackgroundTask;
using Bangk.Domain.Implementation.City;
using Bangk.Domain.Implementation.Client;
using Bangk.Domain.Implementation.Country;
using Bangk.Domain.Implementation.Files;
using Bangk.Domain.Implementation.GroupSettings;
using Bangk.Domain.Implementation.LoanApplication;
using Bangk.Domain.Implementation.LoanApplicationDocument;
using Bangk.Domain.Implementation.LoanApplicationStateHistory;
using Bangk.Domain.Implementation.LoanForm;
using Bangk.Domain.Implementation.LoanQuote;
using Bangk.Domain.Implementation.Login;
using Bangk.Domain.Implementation.Settings;
using Bangk.Domain.Implementation.State;
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
            services.AddSingleton<IValidator<SendCodeValidationEmailDto>, SendCodeValidationEmailValidator>();
            services.AddSingleton<IValidator<CreateClientDto>, CreateClientValidator>();
            services.AddSingleton<IValidator<ClientUpdateInfoDto>, UpdateClientValidator>();
            services.AddSingleton<IValidator<UpdateLoanApplicationDto>, UpdateLoanApplicationValidator>();
            services.AddSingleton<IValidator<CreateUserDto>, CreateUserValidator>();
            // Repositories
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGroupSettingRepository, GroupSettingRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
            services.AddScoped<ILoanApplicationDocumentRepository, LoanApplicationDocumentRepository>();
            services.AddScoped<ILoanApplicationStateHistoryRepository, LoanApplicationStateHistoryRepository>();
            services.AddScoped<ILoanQuoteRepository, LoanQuoteRepository>();
            services.AddScoped<ILoanInsurancePolicyRepository, LoanInsurancePolicyRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<ILoanFormRepository, LoanFormRepository>();
            services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            
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
            services.AddTransient<ISendCodeValidationEmailService, SendCodeValidationEmailService>();
            services.AddTransient<IReSendCodeValidationEmailService, ReSendCodeValidationEmailService>();
            services.AddTransient<ICostumerService, GetClientService>();
            services.AddTransient<ILoanApplicationServices, GetLoanApplicationServices>();
            services.AddTransient<IClientInfoService, GetClientInfoService>();
            services.AddTransient<ICreateClientService, CreateClientService>();
            services.AddTransient<ICreateLoanApplicationService, CreateLoanApplicationService>();
            services.AddTransient<IUpdateClientService, UpdateClientService>();
            services.AddTransient<IGetLoadApplicationDocumentService, GetLoadApplicationDocumentService>();
            services.AddTransient<ILoanApplicationStateHistory, GetLoanApplicationStateHistoryService>();
            services.AddTransient<IGetLoadApplicationByIdService, GetLoadApplicationByIdService>();
            services.AddTransient<IUpdateLoanApplicationService, UpdateLoanApplicationService>();
            services.AddTransient<ILoanQuoteService, LoanQuoteService>();
            services.AddTransient<IGetLoanQuoteInfoByLoanApplicationIdService, GetLoanQuoteInfoByLoanApplicationIdService>();
            services.AddTransient<IAcceptLoanQuoteService, AcceptLoanQuoteService>();
            services.AddTransient<IGetDownloadDocumentService, GetDownloadDocumentService>();
            services.AddTransient<IRejectLoanQuoteService, RejectLoanQuoteService>(); 
            services.AddTransient<IGetAllLoanInsurancePolicyByLoanApplicationIdService, GetAllLoanInsurancePolicyByLoanApplicationIdService>();
            services.AddTransient<IGetCountryService, GetCountryService>();
            services.AddTransient<IGetNationalityService, GetNationalityService>();
            services.AddTransient<IGetStatesService, GetStatesService>();
            services.AddTransient<IGetCityService, GetCityService>();
            services.AddTransient<ICreateLoanInsurancePolicyService, CreateLoanInsurancePolicyService>();
            services.AddTransient<IChangeStateLoanInsurancePolicyService, ChangeStateLoanInsurancePolicyService>();
            services.AddTransient<ILoanSaveFormService, LoanSaveFormService>();
            services.AddTransient<IGetLoanFormService, GetLoanFormService>();
            services.AddTransient<IChangeStateFormsValidatedService, ChangeStateFormsValidatedService>();
            services.AddTransient<ICreateUserService, CreateUserService>(); 

            // Add AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //Azure Blob Storage

            services.AddHostedService<CompleteApplicationJob>();
        }
    }
}