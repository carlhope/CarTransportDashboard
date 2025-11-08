using CarTransportDashboard.Models;

namespace CarTransportDashboard.Tests
{
    public static class TransportJobFactory
    {
        public static TransportJob CreateBasic(
            string? title = null,
            string? description = null,
            string? pickup = null,
            string? dropoff = null,
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

            return new TransportJob(
                title ?? "Test Title",
                description ?? "Test Description",
                pickup ?? "Birmingham",
                dropoff ?? "Manchester",
                scheduledDate ?? DateTime.UtcNow,
                vehicle.Id,
                vehicle,
                status ?? JobStatus.Available
            );
        }
    }
}
