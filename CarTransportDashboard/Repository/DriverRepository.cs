using CarTransportDashboard.Context;
using CarTransportDashboard.Models;
using CarTransportDashboard.Models.Users;
using CarTransportDashboard.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarTransportDashboard.Repository
{
    public class DriverRepository : IDriverRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public DriverRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<DriverProfile?> GetByIdAsync(string id)
        {
            var driverProfile = await _db.DriverProfiles
                .Include(p => p.User)
                .ThenInclude(j=>j.AssignedJobs)
                .FirstOrDefaultAsync(dp => dp.UserId == id);

            return driverProfile;
        }

        public async Task<List<DriverProfile>> GetAllAsync()
        {
            return await _db.DriverProfiles
                .Include(p => p.User)
                .ThenInclude(j=>j.AssignedJobs)
                .ToListAsync();
        }
            

        public async Task<bool> IsInDriverRoleAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user != null && await _userManager.IsInRoleAsync(user, RoleConstants.Driver);
        }

        public async Task<IEnumerable<TransportJob>> GetAssignedJobsAsync(string driverId)
        {
            var results = await _db.TransportJobs
                .Where(j => j.AssignedDriverId == driverId)
                .ToListAsync();
            return results;
        }


    }
}
