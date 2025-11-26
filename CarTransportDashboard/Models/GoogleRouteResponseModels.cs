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

        [JsonPropertyName("legs")]
        public List<Leg> Legs { get; set; } = new();
    }

    public class Leg
    {
        [JsonPropertyName("startLocation")]
        public LatLng StartLocation { get; set; } = new();

        [JsonPropertyName("endLocation")]
        public LatLng EndLocation { get; set; } = new();
    }

    public class LatLng
    {
        [JsonPropertyName("latLng")]
        public LatLngValue Value { get; set; } = new();
    }

    public class LatLngValue
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }


    public class Polyline
    {
        [JsonPropertyName("encodedPolyline")]
        public string EncodedPolyline { get; set; } = string.Empty;
    }
}
