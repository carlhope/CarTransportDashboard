using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Models.Dtos.Users;
using CarTransportDashboard.Models.Users;

public interface IDriverService
{
    Task<List<DriverDto>> GetAllDriversAsync();
    Task<IEnumerable<TransportJobReadDto>> GetAssignedJobsAsync(string driverId);
    Task<DriverDto> GetDriverProfileAsync(string driverId);
    Task<DriverProfile?> GetDriverUserByIdAsync(string driverId);
}