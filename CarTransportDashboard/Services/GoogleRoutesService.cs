using CarTransportDashboard.Models;
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
            _apiKey = config["GoogleMaps:ApiKey"] ?? throw new InvalidOperationException("Google Maps API key missing.");
        }

        public async Task<RouteEstimateDto> GetRouteInfoAsync(string origin, string destination)
        {
            var requestBody = new
            {
                origin = new { address = origin },
                destination = new { address = destination },
                travelMode = "DRIVE",
                routingPreference = "TRAFFIC_AWARE",
                departureTime = DateTime.UtcNow.ToString("o"),
                units = "IMPERIAL"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "directions/v2:computeRoutes")
            {
                Content = JsonContent.Create(requestBody)
            };
            request.Headers.Add("X-Goog-Api-Key", _apiKey);
            request.Headers.Add("X-Goog-FieldMask", "routes.duration,routes.distanceMeters,routes.polyline.encodedPolyline");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var routeResponse = await response.Content.ReadFromJsonAsync<RouteResponse>();
            var route = routeResponse?.Routes.FirstOrDefault() ?? throw new InvalidOperationException("No route found.");

            var durationSeconds = int.Parse(route.Duration.Replace("s", ""));

            return new RouteEstimateDto
            {
                DistanceInMiles = MetersToMiles(route.DistanceMeters),
                EstimatedDuration = TimeSpan.FromSeconds(durationSeconds),
                RoutePreviewUrl = $"https://www.google.com/maps/dir/?api=1&origin={Uri.EscapeDataString(origin)}&destination={Uri.EscapeDataString(destination)}"
            };
        }

        private float MetersToMiles(int meters) => meters / 1609.34f;
    }
}
