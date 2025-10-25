using CarTransportDashboard.Context;
using CarTransportDashboard.Models.Users;
namespace CarTransportDashboard.Models
{
    public class TransportJob
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public JobStatus Status { get; private set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string DropoffLocation { get; set; } = string.Empty;
        public DateTime? ScheduledDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public float DistanceInMiles { get; set; } = (float)(Random.Shared.NextDouble()*(500));
        public decimal CustomerPrice { get; set; }
        public decimal DriverPayment { get; set; }
        public bool isDriveable { get; set; } = true;

        // Foreign Keys
        public Guid? AssignedVehicleId { get; set; }
        public Vehicle? AssignedVehicle { get; set; }

        public string? AssignedDriverId { get; private set; }
        public ApplicationUser? AssignedDriver { get; private set; }

        private decimal basePrice = 100m;
        private float includedMiles = 10.0F;
        private decimal perMileRate = 0.75m;
        private decimal undriveableSurcharge = 50m;
        private float DriverFeePercentage = 0.75f;

        public TransportJob() 
        {
            Status = JobStatus.Available;
        }

        public TransportJob(string title, string description, string pickupLocation, string dropoffLocation, DateTime scheduledDate, Guid assignedVehicleId, Vehicle vehicle)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            PickupLocation = pickupLocation;
            DropoffLocation = dropoffLocation;
            ScheduledDate = scheduledDate;
            AssignedVehicleId = assignedVehicleId;
            AssignedVehicle = vehicle;
            Status = JobStatus.Available;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public TransportJob(string title, string description, string pickupLocation, string dropoffLocation, DateTime scheduledDate, Guid assignedVehicleId, Vehicle vehicle, JobStatus status)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            PickupLocation = pickupLocation;
            DropoffLocation = dropoffLocation;
            ScheduledDate = scheduledDate;
            AssignedVehicleId = assignedVehicleId;
            AssignedVehicle = vehicle;
            Status = status;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }


        public void MarkAsCompleted()
        {
            if (Status != JobStatus.InProgress)
                throw new InvalidOperationException("Only in-progress jobs can be completed.");

            Status = JobStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public void AssignDriver(ApplicationUser driver)
        {
            if (Status != JobStatus.Available)
                throw new InvalidOperationException("Driver can only be assigned to available jobs.");

            AssignedDriver = driver;
            AssignedDriverId = driver.Id;
            Status = JobStatus.Allocated;
            AssignedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UnassignDriver()
        {
            if (Status != JobStatus.Allocated)
                throw new InvalidOperationException("Only allocated jobs can have their driver unassigned.");
            AssignedDriver = null;
            AssignedDriverId = null;
            Status = JobStatus.Available;
            UpdatedAt = DateTime.UtcNow;
        }
        public void AcceptJob()
        {
            if (Status != JobStatus.Allocated)
                throw new InvalidOperationException("Only allocated jobs can be accepted.");
            Status = JobStatus.InProgress;
            AcceptedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Cancel()
        {
            if (Status == JobStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed job.");
            if (Status == JobStatus.Cancelled)
                throw new InvalidOperationException("Job is already cancelled.");

            Status = JobStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
            AssignedDriverId = null;
            AssignedDriver = null;
        }
        private void CalculateCustomerPrice()
        {
            decimal price = basePrice;

            if (DistanceInMiles > includedMiles)
            {
                price += (decimal)(DistanceInMiles - includedMiles) * perMileRate;
            }

            if (!isDriveable)
            {
                price += undriveableSurcharge;
            }

            CustomerPrice=Math.Round(price,2);
            
        }
        private void CalculateDriverFee()
        {
            //ensure customer price is calculated before calculating driver payment
            if (CustomerPrice < basePrice)
            {
                CalculateCustomerPrice();
            }

            DriverPayment = Math.Round(CustomerPrice * 0.75m, 2);
        }
        public void ApplyPricing()
        {
            CalculateCustomerPrice();
            CalculateDriverFee();
        }



    }


}