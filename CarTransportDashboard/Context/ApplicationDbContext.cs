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
        }
    }

}