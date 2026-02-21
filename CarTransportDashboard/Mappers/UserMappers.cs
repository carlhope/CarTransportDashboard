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
        public static AuthUserDto MapFromApplicationUser(ApplicationUser user)
            {
            AuthUserDto target = new AuthUserDto() {
                Id = user.Id,
            };
            return target;
            }
        public static DriverDto MapFromDriverToDriverDto(DriverProfile driver)
        {

            var dto = new DriverDto
            {
 
                UserId = driver.UserId,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpiry = driver.LicenseExpiry,
                FirstName = driver.User.FirstName,
                LastName = driver.User.LastName,
                DisplayName = driver.User.PreferredName ?? $"{driver.User.FirstName} {driver.User.LastName}",
                Email = driver.User.Email!,

            };

            return dto;
        }



    }
}