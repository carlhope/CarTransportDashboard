using CarTransportDashboard.Models.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace CarTransportDashboard.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterDto dto);
        Task<UserDto?> LoginAsync(string email, string password);
        Task<UserDto?> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
        Task<UserDto> FindByEmailAsync(string email);
        Task<UserDto> FindOrCreateByEmailAsync(string email, string? firstName = null, string? lastName = null);
    }
}