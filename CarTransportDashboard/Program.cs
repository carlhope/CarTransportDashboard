using Microsoft.EntityFrameworkCore;
using CarTransportDashboard.Context;
using Microsoft.AspNetCore.Identity;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Services.Interfaces;
using CarTransportDashboard.Repository;
using CarTransportDashboard.Services;



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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularOrigins", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod();
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

app.Run();
