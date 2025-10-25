namespace CarTransportDashboard.Models
{
    public class PricingResult
    {
        public decimal finalCustomerPrice { get; set; }
        public decimal driverFee { get; set; }

        public PricingResult(decimal finalCustomerPrice, decimal driverFee)
        {
            this.finalCustomerPrice = finalCustomerPrice;
            this.driverFee = driverFee;
        }
    }
}
