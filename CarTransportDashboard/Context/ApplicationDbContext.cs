using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarTransportDashboard.Models;
namespace CarTransportDashboard.Context
{


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<TransportJob> TransportJobs => Set<TransportJob>();
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Optional: Fluent API configurations
            builder.Entity<TransportJob>()
                .HasOne(j => j.AssignedVehicle)
                .WithMany(v => v.AssignedJobs)
                .HasForeignKey(j => j.AssignedVehicleId);

            builder.Entity<TransportJob>()
                .HasOne(j => j.AssignedDriver)
                .WithMany(u => u.AssignedJobs)
                .HasForeignKey(j => j.AssignedDriverId);
                // Seed Vehicles
    var vehicle1 = new Vehicle
    {
        Id = Guid.NewGuid(),
        Make = "Ford",
        Model = "Transit",
        RegistrationNumber = "AB12 XYZ"
    };

    var vehicle2 = new Vehicle
    {
        Id = Guid.NewGuid(),
        Make = "Mercedes",
        Model = "Sprinter",
        RegistrationNumber = "CD34 LMN"
    };

    builder.Entity<Vehicle>().HasData(vehicle1, vehicle2);

    // Seed TransportJobs
    builder.Entity<TransportJob>().HasData(
        new TransportJob
        {
            Id = Guid.NewGuid(),
            Title = "Pickup from Manchester",
            Description = "Collect vehicle from Manchester depot",
            Status = JobStatus.Available,
            PickupLocation = "Manchester",
            DropoffLocation = "Liverpool",
            ScheduledDate = DateTime.Today.AddDays(2),
            AssignedVehicleId = vehicle1.Id
        },
        new TransportJob
        {
            Id = Guid.NewGuid(),
            Title = "Delivery to Birmingham",
            Description = "Deliver vehicle to Birmingham client",
            Status = JobStatus.InProgress,
            PickupLocation = "Leeds",
            DropoffLocation = "Birmingham",
            ScheduledDate = DateTime.Today.AddDays(1),
            AssignedVehicleId = vehicle2.Id
        }
    );
        }

    }

}