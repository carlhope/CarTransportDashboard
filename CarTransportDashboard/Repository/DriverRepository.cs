using CarTransportDashboard.Context;
using CarTransportDashboard.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CarTransportDashboard.Repository
{
    public class DriverRepository : IDriverRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DriverRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
            => await _userManager.FindByIdAsync(id);

        public async Task<bool> IsInDriverRoleAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user != null && await _userManager.IsInRoleAsync(user, "Driver");
        }
    }
}
