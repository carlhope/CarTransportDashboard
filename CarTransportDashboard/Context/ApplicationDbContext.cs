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


        }

    }

}