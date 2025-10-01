using CarTransportDashboard.Context;
using CarTransportDashboard.Models.Dtos.Users;
using CarTransportDashboard.Models.Users;

namespace CarTransportDashboard.Mappers
{
    public static class UserMappers
    {
        public static T MapFromApplicationUser<T>(this T target, ApplicationUser user)
         where T : BaseProfileDto
            {
                target.FirstName = user.FirstName;
                target.LastName = user.LastName;
                target.DisplayName = user.PreferredName ?? $"{user.FirstName} {user.LastName}";
                return target;
            }
        public static DriverDto MapFromDriverToDriverDto(DriverProfile driver)
        {
            var dto = new DriverDto
            {
                LicenseNumber = driver.LicenseNumber,
                LicenseExpiry = driver.LicenseExpiry,
                TransportJobs = TransportJobMapper.ToReadDtoList(driver.TransportJobs)
            };
            dto.MapFromApplicationUser(driver.User);
            return dto;
        }


    }
}
