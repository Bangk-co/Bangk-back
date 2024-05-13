using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplicationStateHistory;
using Bangk.Repositories.Repositories;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplicationStateHistory
{
    public class AddCommentService: IAddCommentService
    {
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        public AddCommentService(ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository)
        {
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
        }

        public async Task<ResponseDto<bool>> Execute(int loanApplicationHistoryId, string comment)
        {
            var result = new ResponseDto<bool>
            {
                Result = true
            };

            var history = await _loanApplicationStateHistoryRepository.GetById(loanApplicationHistoryId);
            if (history == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"El histórico Applicacion ingresado no existe");
                result.Result = false;
                return result;
            }

            history.Observations += $", {comment}";
            await _loanApplicationStateHistoryRepository.Update(history);
            return result;
        }
    }
}
