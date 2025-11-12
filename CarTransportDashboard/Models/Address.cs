namespace CarTransportDashboard.Models
{
    public class Address
    {
        public Guid Id { get; set; }
        public string? CompanyName { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string Locality { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string Formatted =>
           string.Join(", ", new[]
           {
                CompanyName,
                AddressLine1,
                AddressLine2,
                Locality,
                PostalCode,
                Country
           }.Where(p => !string.IsNullOrWhiteSpace(p)));


    }
}
