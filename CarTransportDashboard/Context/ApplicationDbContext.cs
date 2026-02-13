using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
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
        public DbSet<DriverProfile> DriverProfiles => Set<DriverProfile>();
        public DbSet<AdminProfile> AdminProfiles => Set<AdminProfile>();
        public DbSet<DispatcherProfile> DispatcherProfiles => Set<DispatcherProfile>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Optional: Fluent API configurations
            builder.Entity<TransportJob>()
                .HasOne(j => j.AssignedVehicle)
                .WithMany(v => v.AssignedJobs)
                .HasForeignKey(j => j.AssignedVehicleId);
            builder.Entity<TransportJob>()
                .OwnsOne(j => j.Polyline);

            builder.Entity<TransportJob>()
                .HasOne(j => j.PickupLocation)
                .WithMany()
                .HasForeignKey(j => j.PickupLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TransportJob>()
                .HasOne(j => j.DropoffLocation)
                .WithMany()
                .HasForeignKey(j => j.DropoffLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TransportJob>(entity =>
            {
                entity.Property(x => x.CustomerPrice)
                      .HasPrecision(18, 2);

                entity.Property(x => x.DriverPayment)
                      .HasPrecision(18, 2);
            });


        }

    }

}