using CarTransportDashboard.Context;
using CarTransportDashboard.Models.Users;
using System.ComponentModel.DataAnnotations;
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
        public float DistanceInMiles { get; set; }
        public decimal CustomerPrice { get; set; }
        public decimal DriverPayment { get; set; }
        public bool isDriveable { get; set; } = true;
        // Pricing Constants
        public static readonly decimal basePrice = 100m;
        public static readonly float includedMiles = 10.0F;
        public static readonly decimal perMileRate = 0.75m;
        public static readonly decimal undriveableSurcharge = 50m;
        public static readonly decimal driverFeePercentage = 0.75m;

        // Foreign Keys
        public Guid? AssignedVehicleId { get; set; }
        public Vehicle? AssignedVehicle { get; set; }

        public string? AssignedDriverId { get; private set; }
        public ApplicationUser? AssignedDriver { get; private set; }

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
        private void ValidateJob()
        {
            if (string.IsNullOrWhiteSpace(Title))
                throw new ValidationException("Job title cannot be empty.");
            if (string.IsNullOrWhiteSpace(PickupLocation) || string.IsNullOrWhiteSpace(DropoffLocation))
                throw new ValidationException("Pickup and dropoff locations must be specified.");
            if (DistanceInMiles <= 0)
                throw new ValidationException("Distance must be greater than zero.");
            if (string.IsNullOrWhiteSpace(Description))
                throw new ValidationException("Job description cannot be empty.");
            if (string.Equals(PickupLocation, DropoffLocation, StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("Pickup and dropoff locations cannot be the same.");
            if (DriverPayment < 25)
            throw new ValidationException("Driver payment must be at least £25.");
            if (CustomerPrice < DriverPayment)
                throw new ValidationException("Customer price must be more than driver payment.");

        }




    }


}