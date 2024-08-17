using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.LoanQuote;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class AcceptLoanQuoteService : IAcceptLoanQuoteService
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ILoanQuoteRepository _loanQuoteRepository;
        private readonly INotificationRepository _notificationRepository;
        public AcceptLoanQuoteService(ILoanApplicationRepository loanApplicationRepository, ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository,
            IClientRepository clientRepository, ILoanQuoteRepository loanQuoteRepository, INotificationRepository notificationRepository)
        {
            _loanApplicationRepository = loanApplicationRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _clientRepository = clientRepository;
            _loanQuoteRepository = loanQuoteRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<ResponseDto<bool>> Execute(int loanApplicationId, DateTime termsConditionsAcceptanceQuote, DateTime acceptQuoteOtp, DateTime sbsPrivacyPolicies)
        {
            var loanApplication = _loanApplicationRepository.GetById(loanApplicationId);
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            var result = new ResponseDto<bool>
            {
                Result = true
            };

            if (loanApplication == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"La Applicacion ingresado no existe");
                result.Result = false;
                return result;
            }

            var loanApplicationHistory = new Repositories.Entities.LoanApplicationStateHistory
            {
                CreationDate = DateTime.UtcNow,
                LoanApplicationId = loanApplicationId,
                Observations = string.Empty,
                StateId = (int)ApplicationStateHistoryEnum.CotizaciónAceptada
            };

            var acceptterms = await _loanQuoteRepository.GetLoanQuoteByLoadApplicationId(loanApplicationId);
            acceptterms.TermsConditionsAcceptanceQuote = termsConditionsAcceptanceQuote;
            acceptterms.AcceptQuoteOtp = acceptQuoteOtp;
            acceptterms.SbsPrivacyPolicies = sbsPrivacyPolicies;
            await _loanQuoteRepository.Update(acceptterms);

            await _loanApplicationStateHistoryRepository.Create(loanApplicationHistory);
            var client = await _clientRepository.GetClientByLoanApplicationId(loanApplicationId);
            client.State = (int)ClientStateEnum.Prospecto;
            await _clientRepository.Update(client);

            var notifications = await _notificationRepository.GetNotificationByApplicationId(loanApplicationId);
            if (notifications != null && notifications.Count > 0)
            {
                await _notificationRepository.DeleteBulk(notifications);
            }

            await CreateNotification(loanApplicationId);
            scope.Complete();
            return result;
        }

        private async Task CreateNotification(int loanApplicationId)
        {
            var notification = new Notification { ApplicationId = loanApplicationId, NotificationType = 1, DateToSendNotification = DateTime.UtcNow.AddHours(1) };
            await _notificationRepository.Create(notification);

            var notification1 = new Notification { ApplicationId = loanApplicationId, NotificationType = 1, DateToSendNotification = DateTime.UtcNow.AddHours(23).AddMinutes(53) };
            await _notificationRepository.Create(notification1);

            var notification2 = new Notification { ApplicationId = loanApplicationId, NotificationType = 1, DateToSendNotification = DateTime.UtcNow.AddDays(3) };
            await _notificationRepository.Create(notification2);

            var notification3 = new Notification { ApplicationId = loanApplicationId, NotificationType = 1, DateToSendNotification = DateTime.UtcNow.AddDays(6) };
            await _notificationRepository.Create(notification3);

            var notification4 = new Notification { ApplicationId = loanApplicationId, NotificationType = 1, DateToSendNotification = DateTime.UtcNow.AddDays(20) };
            await _notificationRepository.Create(notification4);

            var notification5 = new Notification { ApplicationId = loanApplicationId, NotificationType = 2, DateToSendNotification = DateTime.UtcNow.AddDays(30) };
            await _notificationRepository.Create(notification5);
        }
    }
}
