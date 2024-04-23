namespace Bangk.Common.Dto
{
    public class GetLoanQuoteDto
    {
        public string InsuraceCurrierName { get; set; }
        public string LoanTypeName { get; set; }
        public long LoanBalance { get; set; }
        public int LoanApplicationId { get; set; }
        public decimal YearlyFee { get; set;}
        public string ClientName { get; set;}
        public string ClientLastName { get; set;}
        public long? MonthlyCostInsurance { get; set; }
    }
}
