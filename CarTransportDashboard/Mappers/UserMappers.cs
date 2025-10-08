using Azure.Core;
using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos.TransportJob;
using CarTransportDashboard.Models.Dtos.Users;
using CarTransportDashboard.Models.Users;
using System.Data;

namespace CarTransportDashboard.Mappers
{
    public static class UserMappers
    {
        //intended for minimal user info (no roles, tokens, etc)
        public static UserDto MapFromApplicationUser(ApplicationUser user)
            {
            UserDto target = new UserDto() {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.PreferredName ?? $"{user.FirstName} {user.LastName}",
                Email = user.Email!,
            };
            return target;
            }
        public static DriverDto MapFromDriverToDriverDto(DriverProfile driver)
        {

            var dto = new DriverDto
            {
 
                Id = driver.UserId,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpiry = driver.LicenseExpiry,

            };

            return dto;
        }



    }
}