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
              .HasOne(j => j.AssignedDriver)
              .WithMany(dp => dp.TransportJobs)
              .HasForeignKey(j => j.AssignedDriverId);


            builder.Entity<DriverProfile>()
                .HasOne(dp => dp.User)
                .WithOne(u => u.DriverProfile)
                .HasForeignKey<DriverProfile>(dp => dp.UserId);

            builder.Entity<AdminProfile>()
                .HasOne(dp => dp.User)
                .WithOne(u => u.AdminProfile)
                .HasForeignKey<AdminProfile>(dp => dp.UserId);

            builder.Entity<DispatcherProfile>()
                .HasOne(dp => dp.User)
                .WithOne(u => u.DispatcherProfile)
                .HasForeignKey<DispatcherProfile>(dp => dp.UserId);


        }

    }

}