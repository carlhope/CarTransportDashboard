using CarTransportDashboard.Models.Dtos.Routes;

namespace CarTransportDashboard.Services.Interfaces
{
    public interface IRouteService
    {
        Task<RouteEstimateDto> GetRouteInfoAsync(string origin, string destination);


    }
}
