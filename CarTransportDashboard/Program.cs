using Microsoft.EntityFrameworkCore;
using CarTransportDashboard.Context;
using Microsoft.AspNetCore.Identity;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Services.Interfaces;
using CarTransportDashboard.Repository;
using CarTransportDashboard.Services;
using CarTransportDashboard.Models;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
// options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    options.UseInMemoryDatabase("CarTransportDb")); // temporary fix to allow working on mac without SQL Server

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Optional: Customize password, lockout, or user settings here
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<ITransportJobRepository, TransportJobRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<ITransportJobService, TransportJobService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularOrigins", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});


var app = builder.Build();
app.UseCors("AllowAngularOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Ensure database is created
    context.Database.EnsureCreated();

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

        context.SaveChanges();
    }
}


app.Run();


