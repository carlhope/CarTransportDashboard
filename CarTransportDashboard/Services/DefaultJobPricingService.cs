using CarTransportDashboard.Context;
using CarTransportDashboard.Helpers;
using CarTransportDashboard.Models;
using CarTransportDashboard.Services.Interfaces;

namespace CarTransportDashboard.Services
{
    public class DefaultJobPricingService : IJobPriceService
    {
        public PricingResult Calculate(TransportJob job, ApplicationUser customer)
        {
            decimal basePrice = TransportJob.basePrice;

            if (job.DistanceInMiles > TransportJob.includedMiles)
            {
                basePrice += (decimal)(job.DistanceInMiles - TransportJob.includedMiles) * TransportJob.perMileRate;
            }

            if (!job.isDriveable)
            {
                basePrice += TransportJob.undriveableSurcharge;
            }

            decimal discount = 0m;

            //if (customer.HasDiscountEntitlement)
            //{
            //    discount = basePrice * customer.DiscountRate;
            //}

            decimal finalPrice = basePrice - discount;
            decimal driverFee = basePrice * TransportJob.driverFeePercentage;

            return new PricingResult(finalPrice, driverFee);
        }

    }

}
