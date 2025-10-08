using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Models.Dtos.Users;

public interface IDriverService
{
    Task<IEnumerable<TransportJobReadDto>> GetAssignedJobsAsync(string driverId);
    Task<DriverDto> GetDriverProfileAsync(string driverId);
}