using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Services.Interfaces;
namespace CarTransportDashboard.Services
{
    public class TransportJobService : ITransportJobService
    {
        public Task AcceptJobAsync(Guid jobId, string driverId)
        {
            throw new NotImplementedException();
        }

        public Task AssignDriverToJobAsync(Guid jobId, Guid driverId)
        {
            throw new NotImplementedException();
        }

        public Task AssignVehicleToJobAsync(Guid jobId, Guid vehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<TransportJobReadDto> CreateJobAsync(TransportJobWriteDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TransportJobReadDto>> GetAvailableJobsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TransportJobReadDto?> GetJobAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TransportJobReadDto>> GetJobsAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateJobAsync(Guid jobId, TransportJobWriteDto dto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateJobStatusAsync(Guid jobId, JobStatus status)
        {
            throw new NotImplementedException();
        }
    }
}