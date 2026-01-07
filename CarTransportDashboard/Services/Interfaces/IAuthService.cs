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
        IActionResult BeginExternalLogin(string provider, string returnUrl);
        Task<UserDto?> CompleteExternalLoginAsync(string provider, string returnUrl);
        Task<UserDto> LinkExternalProviderAsync(string userId, string provider);
    }
}