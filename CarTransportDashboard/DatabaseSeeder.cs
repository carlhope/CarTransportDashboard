using Microsoft.EntityFrameworkCore;
using CarTransportDashboard.Context;
using Microsoft.AspNetCore.Identity;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Services.Interfaces;
using CarTransportDashboard.Repository;
using CarTransportDashboard.Services;
using CarTransportDashboard.Models;

namespace CarTransportDashboard;

public static class DatabaseSeeder
{
    public static async Task SeedData(IServiceProvider serviceProvider){
        using (var scope = serviceProvider.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();


            // Seed only if empty
            if (!context.TransportJobs.Any())
            {
                var vehicle1 = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    Make = "Ford",
                    Model = "Transit",
                    RegistrationNumber = "AB12XYZ"
                };

                var vehicle2 = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    Make = "Mercedes",
                    Model = "Sprinter",
                    RegistrationNumber = "CD34LMN"
                };

                context.Vehicles.AddRange(vehicle1, vehicle2);

                context.TransportJobs.AddRange(
                   new TransportJob(
                       title: "Pickup from Manchester",
                       description: "Collect vehicle from Manchester depot",
                       pickupLocation: "Manchester",
                       dropoffLocation: "Liverpool",
                       scheduledDate: DateTime.Today.AddDays(2),
                       assignedVehicleId: vehicle1.Id
                   ),
                   new TransportJob(
                       title: "Delivery to Birmingham",
                       description: "Deliver vehicle to Birmingham client",
                       pickupLocation: "Leeds",
                       dropoffLocation: "Birmingham",
                       scheduledDate: DateTime.Today.AddDays(1),
                       assignedVehicleId: vehicle2.Id
                   )
               );

                await context.SaveChangesAsync();
                // Seed roles
                string[] roles = new[] { "Admin", "Driver", "Dispatcher" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
                

                // Seed users
                var adminEmail = "admin@example.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FirstName = "Admin",
                        LastName = "User"
                    };

                    var result = await userManager.CreateAsync(adminUser, "Password123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

               
            }
        }
    }
}