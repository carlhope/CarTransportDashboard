using CarTransportDashboard.Models.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace CarTransportDashboard.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(string, AuthUserDto)> RegisterAsync(RegisterDto dto);
        Task LogoutAsync(string refreshToken);
        Task<AuthUserDto> FindByEmailAsync(string email);
        Task<(string, AuthUserDto?)> RefreshTokenAsync(string refreshToken);
        Task<(string, AuthUserDto?)> LoginAsync(string email, string password);
        Task<(string, AuthUserDto)> FindOrCreateByEmailAsync(string email, string? firstName = null, string? lastName = null);
    }
}