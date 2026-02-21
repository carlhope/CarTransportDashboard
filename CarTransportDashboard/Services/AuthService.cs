using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Dtos;
using CarTransportDashboard.Models.Dtos.Auth;
using CarTransportDashboard.Models.Users;
using CarTransportDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace CarTransportDashboard.Services
{

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, ApplicationDbContext db, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _config = config;
            _db = db;
        }

        public async Task<(string,AuthUserDto)> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                // Add more mappings as needed
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
            // Assign default role
            var roleResult = await _userManager.AddToRoleAsync(user, UserRoles.Driver.ToString());
            if (!roleResult.Succeeded)
                throw new Exception("Failed to assign role: " + string.Join("; ", roleResult.Errors.Select(e => e.Description)));

            var accessToken = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            var csrfToken = GenerateCsrfToken();

            await SaveRefreshTokenAsync(user.Id, refreshToken, csrfToken);
            var roles = await _userManager.GetRolesAsync(user);
            return (csrfToken, MapToUserDto(user, accessToken, refreshToken, roles));
        }

        public async Task<(string,AuthUserDto?)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return (string.Empty, null);
            return await IssueTokensForUserAsync(user);
        }


        public async Task<(string, AuthUserDto?)> RefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _db.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow);

            if (tokenEntity == null || tokenEntity.User == null)
                return (string.Empty, null);

            // Revoke old token
            tokenEntity.IsRevoked = true;
            _db.RefreshTokens.Update(tokenEntity);
            var oldCsrfToken = tokenEntity.CsrfToken;

            var newRefreshToken = GenerateRefreshToken();
            var newAccessToken = await GenerateJwtToken(tokenEntity.User);
            await SaveRefreshTokenAsync(tokenEntity.User.Id, newRefreshToken, oldCsrfToken);

            await _db.SaveChangesAsync();
            var roles = await _userManager.GetRolesAsync(tokenEntity.User);
            return (tokenEntity.CsrfToken, MapToUserDto(tokenEntity.User, newAccessToken, newRefreshToken, roles));


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
        public async Task<AuthUserDto> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;
            return new AuthUserDto
            {
                Id = user.Id,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
            };
        }
        // Lookup or create (for Google login)
        public async Task<(string, AuthUserDto)> FindOrCreateByEmailAsync(string email, string? firstName = null, string? lastName = null)
        {
            var userEntity = await _userManager.FindByEmailAsync(email);
            if (userEntity == null)
            {
                userEntity = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    FirstName = firstName ?? string.Empty,
                    LastName = lastName ?? string.Empty
                };


                var result = await _userManager.CreateAsync(userEntity);
                if (!result.Succeeded)
                    throw new InvalidOperationException("Failed to create user from Google login");
                await _userManager.AddToRoleAsync(userEntity, RoleConstants.Driver);
            }

            return await IssueTokensForUserAsync(userEntity);
        }

        private async Task<(string, AuthUserDto)> IssueTokensForUserAsync(ApplicationUser user)
        {
            var accessToken = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            var csrfToken = GenerateCsrfToken();

            await SaveRefreshTokenAsync(user.Id, refreshToken, csrfToken);
            var roles = await _userManager.GetRolesAsync(user);

            return (csrfToken, MapToUserDto(user, accessToken, refreshToken, roles));
        }
        private async Task SaveRefreshTokenAsync(string userId, string refreshToken, string csrfToken)
        {
            var token = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CsrfToken = csrfToken
            };
            _db.RefreshTokens.Add(token);
            await _db.SaveChangesAsync();
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? "")
        };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }



            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new AuthenticationException("JWT Key is not configured. Check 'Jwt:Key' in appsettings.json or environment variables.");


            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
                throw new AuthenticationException("JWT Issuer or Audience is not configured.");


            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
        private string GenerateCsrfToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }


        private AuthUserDto MapToUserDto(ApplicationUser user, string accessToken, string refreshToken, IList<string> roles)
        {
            return new AuthUserDto
            {
                Id = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Roles = roles.ToList()
            };
        }

        
    }
}