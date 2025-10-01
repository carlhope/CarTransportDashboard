using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;

public interface IDriverService
{
    Task<IEnumerable<TransportJobReadDto>> GetAssignedJobsAsync(string driverId);
}