namespace Bangk.Common.Dto
{
    public class CardPaymentDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public int DocumentIdentificationType { get; set; }
        public string DocumentIdentificationNumber { get; set; }
        public string CardNumber { get; set; }
        public string ExpiredMonth { get; set; }
        public string ExpiredYear { get; set; }
        public string CardHolder { get; set; }
        public int PaymentInstallments { get; set; }
        public string CVC { get; set; }
        public int Amount { get; set; }
    }

    public class CardDto
    {
        public string number { get; set; }
        public string exp_month { get; set; }
        public string exp_year { get; set; }
        public string cvc { get; set; }
        public string card_holder { get; set; }
    }
}
