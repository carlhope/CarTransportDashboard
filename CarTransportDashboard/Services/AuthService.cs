using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos;
using CarTransportDashboard.Models.Dtos.Auth;
using CarTransportDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarTransportDashboard.Models.Users;

namespace CarTransportDashboard.Services
{

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _db = db;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                // Add more mappings as needed
            };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
            // Assign default role
            var roleResult = await _userManager.AddToRoleAsync(user, UserRoles.Driver.ToString());
            if (!roleResult.Succeeded)
                throw new Exception("Failed to assign role: " + string.Join("; ", roleResult.Errors.Select(e => e.Description)));

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            await SaveRefreshTokenAsync(user.Id, refreshToken);

            return new UserDto { Id = user.Id, Email = user.Email!, AccessToken = accessToken, RefreshToken = refreshToken };
        }

        public async Task<UserDto?> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return null;

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            await SaveRefreshTokenAsync(user.Id, refreshToken);

            return new UserDto { Id = user.Id, Email = user.Email!, AccessToken = accessToken, RefreshToken = refreshToken };
        }

        public async Task<UserDto?> RefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _db.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow);

            if (tokenEntity == null || tokenEntity.User == null)
                return null;

            // Revoke old token
            tokenEntity.IsRevoked = true;
            _db.RefreshTokens.Update(tokenEntity);

            var newAccessToken = GenerateJwtToken(tokenEntity.User);
            var newRefreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(tokenEntity.User.Id, newRefreshToken);

            await _db.SaveChangesAsync();

            return new UserDto
            {
                Id = tokenEntity.User.Id,
                Email = tokenEntity.User.Email!,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
        public async Task LogoutAsync(string refreshToken)
        {
            var tokenEntity = await _db.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (tokenEntity != null)
            {
                tokenEntity.IsRevoked = true;
                _db.RefreshTokens.Update(tokenEntity);
                await _db.SaveChangesAsync();
            }
        }
        public async Task<OperationResult<ApplicationUser>> AddUserToRoleAsync(string userId, UserRoles role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new OperationResult<ApplicationUser>(false, "User not found", null);
            if (!await _roleManager.RoleExistsAsync(role.ToString()))
                return new OperationResult<ApplicationUser>(false, $"Role '{role}' does not exist", null);


            var result = await _userManager.AddToRoleAsync(user, role.ToString());
            if (!result.Succeeded)
            {
                var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                return new OperationResult<ApplicationUser>(false, errorMessage, null);
            }

            OperationResult<ApplicationUser> operationResult = new(true, "success",user);
            return operationResult;
        }

        public async Task<OperationResult<ApplicationUser>> RemoveUserFromRoleAsync(string userId, UserRoles role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new OperationResult<ApplicationUser>(false, "User not found", null);

            var result = await _userManager.RemoveFromRoleAsync(user, role.ToString());
            if (!result.Succeeded)
            {
                var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                return new OperationResult<ApplicationUser>(false, errorMessage, null);
            }

            OperationResult<ApplicationUser> operationResult = new(true, "success",user);
            return operationResult;

        }


        private async Task SaveRefreshTokenAsync(string userId, string refreshToken)
        {
            var token = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };
            _db.RefreshTokens.Add(token);
            await _db.SaveChangesAsync();
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? "")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}