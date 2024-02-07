namespace Bangk.Common.Dto
{
    public class UpdateLoanApplicationDto
    {
        public long LoanBalance { get; set; }
        public int LoanTypeId { get; set; }
        public int BankId { get; set; }
        public long LoanNumber { get; set; }
        public long? MonthlyCostInsurance { get; set; }
        public string Observations { get; set; }
        public int LoanApplicationId { get; set; }
        public bool IsValidateLoanApplication { get; set; }
    }
}
