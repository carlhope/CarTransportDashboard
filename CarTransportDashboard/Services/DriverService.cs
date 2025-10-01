using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Mappers;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepo;

    public DriverService(IDriverRepository driverRepo)
    {
        _driverRepo = driverRepo;
    }

    public async Task<IEnumerable<TransportJobReadDto>> GetAssignedJobsAsync(string driverId)
    {
        var jobs = await _driverRepo.GetAssignedJobsAsync(driverId);
        return jobs.Select(TransportJobMapper.ToDto);
    }
}