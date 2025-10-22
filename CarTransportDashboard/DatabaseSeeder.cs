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
                    RegistrationNumber = "AB12XYZ",
                    FuelType=FuelType.Diesel
                };

                var vehicle2 = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    Make = "Mercedes",
                    Model = "Sprinter",
                    RegistrationNumber = "CD34LMN",
                    FuelType = FuelType.Diesel
                };

                context.Vehicles.AddRange(vehicle1, vehicle2);

                context.TransportJobs.AddRange(
                   new TransportJob(
                       title: "Pickup from Manchester",
                       description: "Collect vehicle from Manchester depot",
                       pickupLocation: "Manchester",
                       dropoffLocation: "Liverpool",
                       scheduledDate: DateTime.Today.AddDays(2),
                       assignedVehicleId: vehicle1.Id,
                       vehicle: vehicle1
                   ),
                   new TransportJob(
                       title: "Delivery to Birmingham",
                       description: "Deliver vehicle to Birmingham client",
                       pickupLocation: "Leeds",
                       dropoffLocation: "Birmingham",
                       scheduledDate: DateTime.Today.AddDays(1),
                       assignedVehicleId: vehicle2.Id,
                       vehicle: vehicle2
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
                

                // Seed admin user
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

                // --- New: Seed driver user, vehicles and jobs assigned to the driver ---
                var driverEmail = "driver@example.com";
                var driverUser = await userManager.FindByEmailAsync(driverEmail);
                if (driverUser == null)
                {
                    driverUser = new ApplicationUser
                    {
                        UserName = driverEmail,
                        Email = driverEmail,
                        EmailConfirmed = true,
                        FirstName = "Driver",
                        LastName = "User"
                    };

                    var driverResult = await userManager.CreateAsync(driverUser, "Password123!");
                    if (driverResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(driverUser, "Driver");
                    }
                }

                // Create vehicles dedicated for driver jobs (8 vehicles for 8 jobs)
                var driverVehicles = new List<Vehicle>();
                for (int i = 1; i <= 8; i++)
                {
                    driverVehicles.Add(new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        Make = "DriverMake" + i,
                        Model = "Model" + i,
                        RegistrationNumber = $"DRV{i:D3}",
                        FuelType = i % 2 == 0 ? FuelType.Petrol : FuelType.Diesel
                    });
                }

                context.Vehicles.AddRange(driverVehicles);
                await context.SaveChangesAsync();

                // Create jobs: 3 Completed, 2 InProgress, 3 Available (all assigned to driver as requested)
                var jobs = new List<TransportJob>();

                int index = 0;
                // 3 Completed
                for (int i = 0; i < 3; i++, index++)
                {
                    var v = driverVehicles[index];
                    var job = new TransportJob(
                        title: $"Driver Complete Job {i+1}",
                        description: "Completed job for demo",
                        pickupLocation: $"Origin {i+1}",
                        dropoffLocation: $"Destination {i+1}",
                        scheduledDate: DateTime.Today.AddDays(-i - 1),
                        assignedVehicleId: v.Id,
                        vehicle: v
                    );
                    job.AssignDriver(driverUser);
                    job.AcceptJob();
                    job.MarkAsCompleted();
                    jobs.Add(job);
                }

                // 2 InProgress
                for (int i = 0; i < 2; i++, index++)
                {
                    var v = driverVehicles[index];
                    var job = new TransportJob(
                        title: $"Driver InProgress Job {i+1}",
                        description: "In-progress job for demo",
                        pickupLocation: $"Origin IP {i+1}",
                        dropoffLocation: $"Destination IP {i+1}",
                        scheduledDate: DateTime.Today.AddDays(i),
                        assignedVehicleId: v.Id,
                        vehicle: v
                    );
                    job.AssignDriver(driverUser);
                    job.AcceptJob();
                    jobs.Add(job);
                }

                // 3 Available to driver (Status: Allocated)
                for (int i = 0; i < 3; i++, index++)
                {
                    var v = driverVehicles[index];
                    var job = new TransportJob(
                        title: $"Driver Available Job {i+1}",
                        description: "Available job for demo",
                        pickupLocation: $"Origin A {i+1}",
                        dropoffLocation: $"Destination A {i+1}",
                        scheduledDate: DateTime.Today.AddDays(i + 2),
                        assignedVehicleId: v.Id,
                        vehicle: v
                    );
                    job.AssignDriver(driverUser);
                    jobs.Add(job);
                }

                context.TransportJobs.AddRange(jobs);
                await context.SaveChangesAsync();
            }
        }
    }
}