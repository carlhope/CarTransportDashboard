using CarTransportDashboard.Services.Interfaces;

namespace CarTransportDashboard.Helpers
{
    public static class PricingCalculator
    {
        private static readonly decimal basePrice = 100m;
        private static readonly float includedMiles = 10.0F;
        private static readonly decimal perMileRate = 0.75m;
        private static readonly decimal undriveableSurcharge = 50m;
        private static readonly decimal driverFeePercentage = 0.75m;

        public static decimal CalculateCustomerPrice(float distanceInMiles, bool isDriveable)
        {
            decimal price = basePrice;

            if (distanceInMiles > includedMiles)
            {
                price += (decimal)(distanceInMiles - includedMiles) * perMileRate;
            }

            if (!isDriveable)
            {
                price += undriveableSurcharge;
            }

            return Math.Round(price, 2);
        }

        public static decimal CalculateDriverFee(decimal customerPrice)
        {
            return Math.Round(customerPrice * driverFeePercentage, 2);
        }
    }

}
