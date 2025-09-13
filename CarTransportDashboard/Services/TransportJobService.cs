using CarTransportDashboard.Mappers;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Services.Interfaces;
namespace CarTransportDashboard.Services
{
    public class TransportJobService : ITransportJobService
    {
        private readonly ITransportJobRepository _jobRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IDriverRepository _driverRepo;

        public TransportJobService(
            ITransportJobRepository jobRepo,
            IVehicleRepository vehicleRepo,
            IDriverRepository driverRepo)
        {
            _jobRepo = jobRepo;
            _vehicleRepo = vehicleRepo;
            _driverRepo = driverRepo;
        }

        public async Task<TransportJobReadDto?> GetJobAsync(Guid id)
        {
            var job = await _jobRepo.GetByIdAsync(id);
            return job == null ? null : TransportJobMapper.ToDto(job);
        }

        public async Task<IEnumerable<TransportJobReadDto>> GetJobsAsync()
        {
            var jobs = await _jobRepo.GetAllAsync();
            return jobs.Select(TransportJobMapper.ToDto);
        }

        public async Task<IEnumerable<TransportJobReadDto>> GetAvailableJobsAsync()
        {
            var jobs = await _jobRepo.GetAvailableJobsAsync(); // Filtered by status or unassigned
            return jobs.Select(TransportJobMapper.ToDto);
        }

        public async Task AcceptJobAsync(Guid jobId, string driverId)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            if (job == null) throw new KeyNotFoundException("Job not found");

            job.AssignedDriverId = driverId;
            job.Status = JobStatus.InProgress;
            await _jobRepo.UpdateAsync(job);
        }

        public async Task UpdateJobStatusAsync(Guid jobId, JobStatus status)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            if (job == null) throw new KeyNotFoundException("Job not found");

            job.Status = status;
            await _jobRepo.UpdateAsync(job);
        }

        public async Task AssignVehicleToJobAsync(Guid jobId, Guid vehicleId)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId);
            if (job == null || vehicle == null) throw new KeyNotFoundException("Job or vehicle not found");
            job.AssignedVehicleId = vehicleId;
            await _jobRepo.UpdateAsync(job);
        }

        public async Task AssignDriverToJobAsync(Guid jobId, Guid driverId)
        {
            var jobTask = _jobRepo.GetByIdAsync(jobId);
            var isDriverTask = _driverRepo.IsInDriverRoleAsync(driverId.ToString());
            await Task.WhenAll(jobTask, isDriverTask);
            var job = await jobTask;
            var isDriver = await isDriverTask;

            if (job == null || !isDriver)
                throw new KeyNotFoundException("Job not found or user is not a driver.");

            job.AssignedDriverId = driverId.ToString();
            await _jobRepo.UpdateAsync(job);
        }

        public async Task<TransportJobReadDto> CreateJobAsync(TransportJobWriteDto dto)
        {
            var job = TransportJobMapper.ToModel(dto);
            await _jobRepo.AddAsync(job);
            return TransportJobMapper.ToDto(job);
        }

        public async Task UpdateJobAsync(Guid jobId, TransportJobWriteDto dto)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            if (job == null) throw new KeyNotFoundException("Job not found");
            TransportJobMapper.UpdateModel(job, dto);
            await _jobRepo.UpdateAsync(job);
        }
    }
}