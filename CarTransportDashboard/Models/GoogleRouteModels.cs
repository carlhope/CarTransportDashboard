namespace CarTransportDashboard.Models
{
    public class RouteResponse
    {
        public List<Route> Routes { get; set; } = new();
    }

    public class Route
    {
        public string Duration { get; set; } = string.Empty;
        public int DistanceMeters { get; set; }
        public Polyline Polyline { get; set; } = new();
    }

    public class Polyline
    {
        public string EncodedPolyline { get; set; } = string.Empty;
    }
}
