namespace CarTransportDashboard.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userName = context.User.Identity?.IsAuthenticated == true
                ? context.User.Identity.Name
                : "Anonymous";
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

            _logger.LogInformation("Handling request: {Method} {Path} by {User} from {Ip}",
                context.Request.Method, context.Request.Path, userName, ipAddress);
            _logger.LogInformation("Finished handling request. Status: {StatusCode} by {User} from {Ip}",
                context.Response.StatusCode, userName, ipAddress);

            await _next(context);

            _logger.LogInformation("Finished handling request. Status: {StatusCode} by {User}",
                context.Response.StatusCode, userName);
        }
    }
}
