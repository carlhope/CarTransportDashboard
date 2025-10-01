using CarTransportDashboard.Context;

namespace CarTransportDashboard.Models.Users
{
    public interface IHasApplicationUser
    {
        ApplicationUser User { get; }
    }
}
