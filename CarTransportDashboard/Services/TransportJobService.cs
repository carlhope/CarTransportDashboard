using CarTransportDashboard.Helpers;
using CarTransportDashboard.Mappers;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Models.Dtos.Vehicle;
using CarTransportDashboard.Repository.Interfaces;
using CarTransportDashboard.Services.Interfaces;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
namespace CarTransportDashboard.Services
{
    public class TransportJobService : ITransportJobService
    {
        private readonly ITransportJobRepository _jobRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IDriverRepository _driverRepo;
        private readonly IDriverService _driverService;
        private readonly ILogger<TransportJobService> _logger;
        private readonly IRouteService _routeService;
        private readonly IEmailService _emailService;


        public TransportJobService(
            ITransportJobRepository jobRepo,
            IVehicleRepository vehicleRepo,
            IDriverRepository driverRepo,
            IDriverService driverService,
            ILogger<TransportJobService> logger,
            IRouteService routeService,
            IEmailService emailService)
        {
            _jobRepo = jobRepo;
            _vehicleRepo = vehicleRepo;
            _driverRepo = driverRepo;
            _driverService = driverService;
            _logger = logger;
            _routeService = routeService;
            _emailService = emailService;
        }

        public async Task<TransportJobReadDto?> GetJobAsync(Guid id)
        {
            var job = await _jobRepo.GetByIdAsync(id);
            return job == null ? null : TransportJobMapper.ToDto(job);
        }

        public async Task<TransportJob?> GetJobEntityAsync(Guid id)
        {
            return await _jobRepo.GetByIdAsync(id);
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
            //await UpdateRouteInfoAsync(job); //commented out to avoid api overuse whilst in development. can be re-enabled later if needed.
            job.AcceptJob();

            var jobDto = TransportJobMapper.ToDto(job);

            await _jobRepo.UpdateAsync(job);
            if (job.AssignedDriverId != null)
            {
                var email = new Email(
                     RecipientUserId: job.AssignedDriverId,
                     EmailType: EmailType.JobAccepted,
                     SenderUserId: "" //TODO: refactor to add parameter for sender user id. this can be obtained from auth context in controller layer.
                 );

                await _emailService.SendEmailAsync(email);

            }
            return OperationResult<TransportJobReadDto>.CreateSuccess(jobDto, "Job accepted successfully.");
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
            var isDriverTask = _driverRepo.IsInDriverRoleAsync(driverId);
            var driverTask = _driverService.GetDriverUserByIdAsync(driverId);

            await Task.WhenAll(jobTask, isDriverTask, driverTask);
            var job = await jobTask;
            var isDriver = await isDriverTask;
            var driver = await driverTask;

            if (job is null || driver is null || !isDriver)
                return OperationResult<TransportJobReadDto>.CreateFailure("Job not found or user is not a driver.");

            try
            {
                job.AssignDriver(driver);
                await UpdateRouteInfoAsync(job);
                var result = await _jobRepo.UpdateAsync(job);

                if (!result.Success)
                    return OperationResult<TransportJobReadDto>.CreateFailure(result.Message);

                var jobDto = TransportJobMapper.ToDto(result.Data!);
                if (job.AssignedDriverId != null)
                {
                    var email = new Email(
                         RecipientUserId: job.AssignedDriverId,
                         EmailType: EmailType.JobAssigned,
                         SenderUserId: "" //TODO: refactor to add parameter for sender user id. this can be obtained from auth context in controller layer.
                     );

                    await _emailService.SendEmailAsync(email);

                }
                return OperationResult<TransportJobReadDto>.CreateSuccess(jobDto, "Driver assigned to job successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return OperationResult<TransportJobReadDto>.CreateFailure(ex.Message);
            }
        }


        public async Task<OperationResult<TransportJobReadDto>> CreateJobAsync(TransportJobCreateDto dto)
        {
            if (dto.AssignedVehicleId.HasValue)
            {
                dto.AssignedVehicle = null; 
            }
            var job = TransportJobMapper.ToModel(dto);
            await UpdateRouteInfoAsync(job);
            if (IsSuspiciousRoute(job))
            {
                //TODO: implement notification to dispatch team for review & inform creator
            }

            job.CalculatePricing();
            try
            {
                job.Validate();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Job validation failed: {Message}", ex.Message);
                return OperationResult<TransportJobReadDto>.CreateFailure(ex.Message);
            }
            var result = await _jobRepo.AddAsync(job);

            if (!result.Success || result.Data is null)
                return OperationResult<TransportJobReadDto>.CreateFailure(result.Message);

            var readDto = TransportJobMapper.ToDto(result.Data);
            return OperationResult<TransportJobReadDto>.CreateSuccess(readDto, result.Message);
        }

        public async Task<OperationResult<TransportJobReadDto>> UpdateJobAsync(Guid jobId, TransportJobUpdateDto dto)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);
            if (job is null)
                return OperationResult<TransportJobReadDto>.CreateFailure("Transport job not found.");

            TransportJobMapper.UpdateModel(job, dto);
            await UpdateRouteInfoAsync(job);
            if (IsSuspiciousRoute(job))
            {
                //TODO: implement notification to dispatch team for review & inform creator
            }
            job.CalculatePricing();
            try
            {
                job.Validate();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Post-update Job validation failed: {Message}", ex.Message);
                return OperationResult<TransportJobReadDto>.CreateFailure(ex.Message);
            }
            var updateResult = await _jobRepo.UpdateAsync(job);

            if (!updateResult.Success || updateResult.Data is null)
                return OperationResult<TransportJobReadDto>.CreateFailure(updateResult.Message);

            var readDto = TransportJobMapper.ToDto(updateResult.Data);
            return OperationResult<TransportJobReadDto>.CreateSuccess(readDto, "Job updated successfully.");
        }

        public async Task<OperationResult<TransportJobReadDto>> CompleteJobAsync(TransportJob job)
        {
            if (job == null)
                return OperationResult<TransportJobReadDto>.CreateFailure("Job not found.");

            try
            {
                job.MarkAsCompleted();
                var result = await _jobRepo.UpdateAsync(job);

                if (!result.Success)
                    return OperationResult<TransportJobReadDto>.CreateFailure(result.Message);

                var jobDto = TransportJobMapper.ToDto(result.Data!);
                return OperationResult<TransportJobReadDto>.CreateSuccess(jobDto, "Job marked as completed.");
            }
            catch (InvalidOperationException ex)
            {
                return OperationResult<TransportJobReadDto>.CreateFailure(ex.Message);
            }
        }


        public async Task<OperationResult<TransportJobReadDto>> UnassignDriverFromJobAsync(TransportJob job)
        {
            if (job.AssignedDriverId == null)
                return OperationResult<TransportJobReadDto>.CreateFailure("Job has no assigned driver.");
            job.UnassignDriver();

            var result = await _jobRepo.UpdateAsync(job);
            if (!result.Success) return OperationResult<TransportJobReadDto>.CreateFailure(result.Message);
            if(result.Data == null) return OperationResult<TransportJobReadDto>.CreateFailure("Job is null");
            var dto = TransportJobMapper.ToDto(result.Data);
            return OperationResult<TransportJobReadDto>.CreateSuccess(dto);
        }


        public async Task<OperationResult<TransportJobReadDto>> CancelJob(TransportJob job)
        {
            try
            {
                job.Cancel();
                //future: add logic to notify assigned driver, update related schedules, etc.
                var result = await _jobRepo.UpdateAsync(job);
                return result.Success
                    ? OperationResult<TransportJobReadDto>.CreateSuccess(TransportJobMapper.ToDto(result.Data!), "Job cancelled.")
                    : OperationResult<TransportJobReadDto>.CreateFailure(result.Message);
            }
            catch (InvalidOperationException ex)
            {
                return OperationResult<TransportJobReadDto>.CreateFailure(ex.Message);
            }
        }
        private async Task UpdateRouteInfoAsync(TransportJob job)
        {
            var route = await _routeService.GetRouteInfoAsync(job.PickupLocation.PostalCode, job.DropoffLocation.PostalCode);
            job.DistanceInMiles = route.DistanceInMiles;
            job.EstimatedDuration = route.EstimatedDuration;
            job.RoutePreviewUrl = route.RoutePreviewUrl;
            job.LastRouteEstimateTime = DateTime.UtcNow;
        }

        private bool IsSuspiciousRoute(TransportJob job)
        {
            //catches unusually long routes for further review. possible address data misinterpretation by geocoding service.
            //example misinterpretation: "Birmingham, UK" vs "Birmingham, AL, USA"
            //if job is legitimate but flagged, dispatchers can manually review and clear the flag.
            if( job.DistanceInMiles > 500 || job.EstimatedDuration.TotalHours > 10)
                {
                    job.IsRouteSuspicious = true;
                    _logger.LogWarning("Job {JobId} flagged as suspicious route: {Distance} miles, {Duration} hours",
                        job.Id, job.DistanceInMiles, job.EstimatedDuration.TotalHours);
                }
                else
                {
                    job.IsRouteSuspicious = false;
                }
            return job.IsRouteSuspicious;
        }

    }
}