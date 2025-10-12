using CarTransportDashboard.Mappers;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Models.Users;
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
        public async Task<IEnumerable<TransportJobReadDto>> GetJobsByDriverIdAsync(List<string> id, string? status, DateTime? startDate =null )
        {
            //drivers can search for their own jobs. Admin and dispatchers can pass any array of driver ids via their own api endpoints.
            var jobs = await _jobRepo.GetAllByDriverIdsAsync(id, status!, startDate);
            return jobs.Select(TransportJobMapper.ToDto);
        }

        public async Task<IEnumerable<TransportJobReadDto>> GetAvailableJobsAsync()
        {
            var jobs = await _jobRepo.GetAvailableJobsAsync(); // Filtered by status or unassigned
            return jobs.Select(TransportJobMapper.ToDto);
        }

        public async Task<OperationResult<TransportJobReadDto>> AcceptJobAsync(Guid jobId, string driverId)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            if (job is null)
                return OperationResult<TransportJobReadDto>.CreateFailure("Transport job not found.");

            if (job.Status != JobStatus.Allocated || job.AssignedDriverId!=driverId)
                return OperationResult<TransportJobReadDto>.CreateFailure("Job is not available for acceptance.");

            job.Status = JobStatus.InProgress;

            var jobDto = TransportJobMapper.ToDto(job);

            await _jobRepo.UpdateAsync(job);
            return OperationResult<TransportJobReadDto>.CreateSuccess(jobDto, "Job accepted successfully.");
        }

        public async Task<OperationResult<TransportJobReadDto>> UpdateJobStatusAsync(Guid jobId, JobStatus status)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            if (job is null)
                return OperationResult<TransportJobReadDto>.CreateFailure("Transport job not found.");

            job.Status = status;
            await _jobRepo.UpdateAsync(job);
            var jobDto = TransportJobMapper.ToDto(job);
            return OperationResult<TransportJobReadDto>.CreateSuccess(jobDto, "Job status updated successfully.");
        }

        public async Task<OperationResult<TransportJobReadDto>> AssignVehicleToJobAsync(Guid jobId, Guid vehicleId)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId);

            if (job is null || vehicle is null)
                return OperationResult<TransportJobReadDto>.CreateFailure("Job or vehicle not found.");

            job.AssignedVehicleId = vehicleId;
            await _jobRepo.UpdateAsync(job);
            var jobDto = TransportJobMapper.ToDto(job);
            return OperationResult<TransportJobReadDto>.CreateSuccess(jobDto, "Vehicle assigned to job successfully.");
        }

        public async Task<OperationResult<TransportJobReadDto>> AssignDriverToJobAsync(Guid jobId, string driverId)
        {
            var jobTask = _jobRepo.GetByIdAsync(jobId);
            var isDriverTask = _driverRepo.IsInDriverRoleAsync(driverId.ToString());

            await Task.WhenAll(jobTask, isDriverTask);
            var job = await jobTask;
            var isDriver = await isDriverTask;

            if (job is null || !isDriver)
                return OperationResult<TransportJobReadDto>.CreateFailure("Job not found or user is not a driver.");
            if(job.Status != JobStatus.Available)
                return OperationResult<TransportJobReadDto>.CreateFailure("Job is not available for assignment.");
            if (job.Status == JobStatus.Available)
            {
                job.AssignedDriverId = driverId;
                job.Status = JobStatus.Allocated;
                await _jobRepo.UpdateAsync(job);
                var jobDto = TransportJobMapper.ToDto(job);
                return OperationResult<TransportJobReadDto>.CreateSuccess(jobDto, "Driver assigned to job successfully.");
            }
            else
            {
                return OperationResult<TransportJobReadDto>.CreateFailure("Job is not available for assignment.");
            }
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

        public Task<OperationResult<TransportJobReadDto>> DeleteJobAsync(Guid jobId)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<TransportJobReadDto>> CompleteJobAsync(Guid jobId)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<TransportJobReadDto>> UnassignDriverFromJobAsync(Guid jobId, string actorId, UserRoles actorRole)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<TransportJobReadDto>> CancelJob(Guid jobId)
        {
            throw new NotImplementedException();
        }
    }
}