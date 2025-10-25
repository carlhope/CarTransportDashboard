using CarTransportDashboard.Models;
using System.ComponentModel.DataAnnotations;

namespace CarTransportDashboard.Helpers
{
    public static class TransportJobValidator
    {
        public static void Validate(TransportJob job)
        {
            if (string.IsNullOrWhiteSpace(job.Title))
                throw new ValidationException("Title is required.");

            if (job.ScheduledDate == null || job.ScheduledDate < DateTime.UtcNow)
                throw new ValidationException("Scheduled date must be in the future.");

            if (job.DistanceInMiles <= 0)
                throw new ValidationException("Distance must be greater than zero.");
        }
    }

}
