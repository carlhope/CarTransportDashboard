using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;

namespace CarTransportDashboard.Mappers
{
    public static class TransportJobMapper
    {
        public static TransportJobReadDto ToDto(TransportJob job)
        {
            return new TransportJobReadDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                PickupLocation = job.PickupLocation,
                DropoffLocation = job.DropoffLocation,
                ScheduledDate = job.ScheduledDate,
                Status = job.Status,
                AssignedDriverId = job.AssignedDriverId,
                AssignedVehicleId = job.AssignedVehicleId,
                CreatedAt = job.CreatedAt,
                UpdatedAt = job.UpdatedAt,
                AssignedAt = job.AssignedAt,
                CompletedAt = job.CompletedAt,
                AcceptedAt = job.AcceptedAt
            };
        }
        public static List<TransportJobReadDto> ToReadDtoList(IEnumerable<TransportJob> jobs)
        {
            return jobs.Select(j => ToDto(j)).ToList()?? new List<TransportJobReadDto>();
        }

        public static TransportJob ToModel(TransportJobWriteDto dto)
        {
            return new TransportJob(
                   title: dto.Title,
                   description: dto.Description,
                   pickupLocation: dto.PickupLocation,
                   dropoffLocation: dto.DropoffLocation,
                   scheduledDate: dto.ScheduledDate ?? DateTime.UtcNow,
                   assignedVehicleId: dto.AssignedVehicleId ?? Guid.Empty
               )
               {
                   Id = dto.Id,
                   AssignedDriverId = dto.AssignedDriverId
               };

        }

        public static void UpdateModel(TransportJob job, TransportJobWriteDto dto)
        {
            job.Title = dto.Title;
            job.Description = dto.Description;
            job.PickupLocation = dto.PickupLocation;
            job.DropoffLocation = dto.DropoffLocation;
            job.ScheduledDate = dto.ScheduledDate;
            job.AssignedDriverId = dto.AssignedDriverId;
            job.AssignedVehicleId = dto.AssignedVehicleId;
        }
    }

}
