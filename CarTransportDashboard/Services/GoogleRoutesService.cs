using CarTransportDashboard.Models.Dtos.Routes;
using CarTransportDashboard.Services.Interfaces;
using System.Text.Json;

namespace CarTransportDashboard.Services
{
    public class GoogleRoutesService : IRouteService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GoogleRoutesService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["GoogleMaps:ApiKey"];
        }

        public async Task<RouteEstimateDto> GetRouteInfoAsync(string origin, string destination)
        {
            var url = $"https://routes.googleapis.com/directions/v2:computeRoutes?key={_apiKey}";

            var requestBody = new
            {
                origin = new { address = origin },
                destination = new { address = destination },
                travelMode = "DRIVE",
                routingPreference = "TRAFFIC_AWARE",
                departureTime = DateTime.UtcNow.ToString("o"),
                units = "IMPERIAL"
            };

            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var route = json.GetProperty("routes")[0];
            var distanceMeters = route.GetProperty("distanceMeters").GetInt32();
            var durationSeconds = route.GetProperty("duration").GetProperty("seconds").GetInt32();

            return new RouteEstimateDto
            {
                DistanceInMiles = distanceMeters / 1609.34f,
                EstimatedDuration = TimeSpan.FromSeconds(durationSeconds),
                RoutePreviewUrl = $"https://www.google.com/maps/dir/?api=1&origin={Uri.EscapeDataString(origin)}&destination={Uri.EscapeDataString(destination)}"
            };
        }
    }
}
