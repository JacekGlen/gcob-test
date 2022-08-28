namespace Rabobank.TechnicalTest.GCOB.Dtos
{
    public class CustomerCreateRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
    }
}
