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
            var requestBody = new RouteRequest
            {
                Origin = new Location { Address = origin },
                Destination = new Location { Address = destination },
                TravelMode = "DRIVE",
                RoutingPreference = "TRAFFIC_AWARE",
                DepartureTime = DateTime.UtcNow.ToString("o"),
                Units = "IMPERIAL",
                RouteModifiers = new RouteModifiers
                { 
                    AvoidTolls = true, //removes requirement to factor toll costs into job pricing. drivers can still choose to take toll roads if they wish.
                }
            };
            _httpClient.DefaultRequestHeaders.Add("X-Goog-Api-Key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("X-Goog-FieldMask", "routes.duration,routes.distanceMeters,routes.polyline.encodedPolyline");

            var response = await _httpClient.PostAsync(
                "https://routes.googleapis.com/directions/v2:computeRoutes",
                JsonContent.Create(requestBody)
                );
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
