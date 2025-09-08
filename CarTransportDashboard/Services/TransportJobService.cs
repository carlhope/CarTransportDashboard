using CarTransportDashboard.Models;
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

        public Task<IEnumerable<TransportJob>> GetAvailableJobsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TransportJob?> GetJobAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TransportJob>> GetJobsAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateJobStatusAsync(Guid jobId, JobStatus status)
        {
            throw new NotImplementedException();
        }
    }
}