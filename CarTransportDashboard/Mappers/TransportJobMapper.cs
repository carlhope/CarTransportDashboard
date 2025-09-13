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
                PickupLocation = job.PickupLocation,
                DropoffLocation = job.DropoffLocation,
                ScheduledDate = job.ScheduledDate,
                Status = job.Status,
                AssignedDriverId = job.AssignedDriverId,
                AssignedVehicleId = job.AssignedVehicleId
            };
        }

        public static TransportJob ToModel(TransportJobWriteDto dto)
        {
            return new TransportJob
            {
                Id = dto.Id,
                PickupLocation = dto.PickupLocation,
                DropoffLocation = dto.DropoffLocation,
                ScheduledDate = dto.ScheduledDate,
                Status = dto.Status,
                AssignedDriverId = dto.AssignedDriverId,
                AssignedVehicleId = dto.AssignedVehicleId
            };
        }

        public static void UpdateModel(TransportJob job, TransportJobWriteDto dto)
        {
            job.PickupLocation = dto.PickupLocation;
            job.DropoffLocation = dto.DropoffLocation;
            job.ScheduledDate = dto.ScheduledDate;
            job.Status = dto.Status;
            job.AssignedDriverId = dto.AssignedDriverId;
            job.AssignedVehicleId = dto.AssignedVehicleId;
        }
    }

}
