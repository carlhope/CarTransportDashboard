namespace CarTransportDashboard.Models.Dtos.Routes
{
    public class RouteEstimateDto
    {
        public float DistanceInMiles { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
        public string RoutePreviewUrl { get; set; }
        public Polyline Polyline { get; set; } = new();

    }
}
