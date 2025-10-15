using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Mappers;
using CarTransportDashboard.Models.Dtos.Users;
using CarTransportDashboard.Models.Users;
using CarTransportDashboard.Context;
using Microsoft.AspNetCore.Identity;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepo;
    private readonly UserManager<ApplicationUser> _userManager;

    public DriverService(IDriverRepository driverRepo, UserManager<ApplicationUser> userManager)
    {
        _driverRepo = driverRepo;
        _userManager = userManager;
    }

    public async Task<IEnumerable<TransportJobReadDto>> GetAssignedJobsAsync(string driverId)
    {
        var jobs = await _driverRepo.GetAssignedJobsAsync(driverId);
        return jobs.Select(TransportJobMapper.ToDto);
    }

    public async Task<DriverDto> GetDriverProfileAsync(string driverId)
    {
        DriverProfile driver = await _driverRepo.GetByIdAsync(driverId);
        return UserMappers.MapFromDriverToDriverDto(driver);
    }
    public async Task<ApplicationUser?> GetDriverUserByIdAsync(string driverId)
    {
        ApplicationUser driver = await _userManager.FindByIdAsync(driverId);
        return driver;
    }
}