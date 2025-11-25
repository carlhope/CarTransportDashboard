using System.Text.Json.Serialization;

namespace CarTransportDashboard.Models
{
    public class RouteResponse
    {
        [JsonPropertyName("routes")]
        public List<Route> Routes { get; set; } = new();
    }

    public class Route
    {
        [JsonPropertyName("duration")]
        public string Duration { get; set; } = string.Empty;

        [JsonPropertyName("distanceMeters")]
        public int DistanceMeters { get; set; }

        [JsonPropertyName("polyline")]
        public Polyline Polyline { get; set; } = new();
    }

    public class Polyline
    {
        [JsonPropertyName("encodedPolyline")]
        public string EncodedPolyline { get; set; } = string.Empty;
    }
}
