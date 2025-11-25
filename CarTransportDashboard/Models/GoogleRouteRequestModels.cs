using System.Text.Json.Serialization;

namespace CarTransportDashboard.Models
{

    public class RouteRequest
    {
        [JsonPropertyName("origin")]
        public Location Origin { get; set; }

        [JsonPropertyName("destination")]
        public Location Destination { get; set; }

        [JsonPropertyName("travelMode")]
        public string TravelMode { get; set; } = "DRIVE";

        [JsonPropertyName("routingPreference")]
        public string RoutingPreference { get; set; } = "TRAFFIC_AWARE";

        [JsonPropertyName("departureTime")]
        public string DepartureTime { get; set; } = DateTime.UtcNow.ToString("o");

        [JsonPropertyName("units")]
        public string Units { get; set; } = "IMPERIAL";

        [JsonPropertyName("routeModifiers")]
        public RouteModifiers RouteModifiers { get; set; } = new RouteModifiers { AvoidTolls = true };
    }

    public class Location
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }
    }

    public class RouteModifiers
    {
        [JsonPropertyName("avoidTolls")]
        public bool AvoidTolls { get; set; }

        [JsonPropertyName("avoidFerries")]
        public bool AvoidFerries { get; set; } = false;

        [JsonPropertyName("avoidHighways")]
        public bool AvoidHighways { get; set; } = false;
    }
}
