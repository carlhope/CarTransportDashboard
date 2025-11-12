using CarTransportDashboard.Models;

namespace CarTransportDashboard.Tests
{
    public static class TransportJobFactory
    {
        public static TransportJob CreateBasic(
            string? title = null,
            string? description = null,
            Address? pickup = null,
            Address? dropoff = null,
            DateTime? scheduledDate = null,
            JobStatus? status = null)
        {
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                Make = "TestMake",
                Model = "TestModel",
                RegistrationNumber = "TEST123",
                FuelType = FuelType.Petrol
            };
            var mockPickupAddress = new Address
            {
                CompanyName = "Acme Supplies Ltd",
                AddressLine1 = "Unit 4, Acme Business Park",
                AddressLine2 = "Warehouse Entrance",
                Locality = "Stoke-on-Trent",
                PostalCode = "ST1 1AA",
                Country = "GB",
                Lat = 53.0027,
                Lng = -2.1794
            };

            var mockDropoffAddress = new Address
            {
                CompanyName = "Derby Distribution Hub",
                AddressLine1 = "456 Industrial Estate",
                AddressLine2 = "Loading Bay 3",
                Locality = "Derby",
                PostalCode = "DE1 2BB",
                Country = "GB",
                Lat = 52.9225,
                Lng = -1.4746
            };


            return new TransportJob(
                title ?? "Test Title",
                description ?? "Test Description",
                pickup ?? mockPickupAddress,
                dropoff ?? mockDropoffAddress,
                scheduledDate ?? DateTime.UtcNow,
                vehicle.Id,
                vehicle,
                status ?? JobStatus.Available
            );
        }
    }
}
