using CarTransportDashboard.Context;
using CarTransportDashboard.Models;

namespace CarTransportDashboard.Services.Interfaces
{
    public interface IJobPriceService
    {
            PricingResult Calculate(TransportJob job, ApplicationUser customer = null);
        

    }
}
