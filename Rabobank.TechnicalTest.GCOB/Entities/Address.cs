namespace Rabobank.TechnicalTest.GCOB.Entities
{
    public sealed class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public int CountryId { get; set; }
    }
}
