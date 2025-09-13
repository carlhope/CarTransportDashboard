using CarTransportDashboard.Context;

namespace CarTransportDashboard.Repository.Interfaces
{
    public interface IDriverRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<bool> IsInDriverRoleAsync(string id);

    }
}
