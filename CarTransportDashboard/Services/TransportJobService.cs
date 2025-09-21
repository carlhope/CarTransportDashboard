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

        public async Task<OperationResult<TransportJob>> AcceptJobAsync(Guid jobId, string driverId)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            if (job is null)
                return OperationResult<TransportJob>.CreateFailure("Transport job not found.");

            job.AssignedDriverId = driverId;
            job.Status = JobStatus.InProgress;

            return await _jobRepo.UpdateAsync(job);
        }

        public async Task<OperationResult<TransportJob>> UpdateJobStatusAsync(Guid jobId, JobStatus status)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            if (job is null)
                return OperationResult<TransportJob>.CreateFailure("Transport job not found.");

            job.Status = status;
            return await _jobRepo.UpdateAsync(job);
        }

        public async Task<OperationResult<TransportJob>> AssignVehicleToJobAsync(Guid jobId, Guid vehicleId)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId);

            if (job is null || vehicle is null)
                return OperationResult<TransportJob>.CreateFailure("Job or vehicle not found.");

            job.AssignedVehicleId = vehicleId;
            return await _jobRepo.UpdateAsync(job);
        }

        public async Task<OperationResult<TransportJob>> AssignDriverToJobAsync(Guid jobId, string driverId)
        {
            var jobTask = _jobRepo.GetByIdAsync(jobId);
            var isDriverTask = _driverRepo.IsInDriverRoleAsync(driverId.ToString());

            await Task.WhenAll(jobTask, isDriverTask);
            var job = await jobTask;
            var isDriver = await isDriverTask;

            if (job is null || !isDriver)
                return OperationResult<TransportJob>.CreateFailure("Job not found or user is not a driver.");

            job.AssignedDriverId = driverId;
            return await _jobRepo.UpdateAsync(job);
        }

        public async Task<OperationResult<TransportJobReadDto>> CreateJobAsync(TransportJobWriteDto dto)
        {
            var job = TransportJobMapper.ToModel(dto);
            var result = await _jobRepo.AddAsync(job);

            if (!result.Success || result.Data is null)
                return OperationResult<TransportJobReadDto>.CreateFailure(result.Message);

            var readDto = TransportJobMapper.ToDto(result.Data);
            return OperationResult<TransportJobReadDto>.CreateSuccess(readDto, result.Message);
        }

        public async Task<OperationResult<TransportJobReadDto>> UpdateJobAsync(Guid jobId, TransportJobWriteDto dto)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            if (job is null)
                return OperationResult<TransportJobReadDto>.CreateFailure("Transport job not found.");

            TransportJobMapper.UpdateModel(job, dto);
            var updateResult = await _jobRepo.UpdateAsync(job);

            if (!updateResult.Success || updateResult.Data is null)
                return OperationResult<TransportJobReadDto>.CreateFailure(updateResult.Message);

            var readDto = TransportJobMapper.ToDto(updateResult.Data);
            return OperationResult<TransportJobReadDto>.CreateSuccess(readDto, "Job updated successfully.");
        }
    }
}