using CarTransportDashboard.Models.Dtos.Auth;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterDto dto);
    Task<UserDto?> LoginAsync(string email, string password);
    Task<UserDto?> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string refreshToken); // Add this
}