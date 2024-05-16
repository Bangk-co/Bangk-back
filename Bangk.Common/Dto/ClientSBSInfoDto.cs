namespace Bangk.Common.Dto
{
    public class ClientSBSInfoDto
    {
        public int BirthCountryId { get; set; }
        public int CountryNationalityId { get; set; }
        public int BirthCountrySBSId { get; set; }
        public int CountryNationalitySBSId { get; set; }
    }

    public class ClientSarlaftInfoDto
    {
        public long LoanBalance { get; set; }
        public bool IsPublicPerson { get; set; }
        public bool HasBondPublicPerson { get; set; }
        public decimal YearlyFee { get; set; }
    }
}
