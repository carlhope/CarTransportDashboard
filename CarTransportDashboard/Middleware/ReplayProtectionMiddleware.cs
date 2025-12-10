using Microsoft.Extensions.Caching.Memory;

namespace CarTransportDashboard.Middleware
{
    public class ReplayProtectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _nonceCache;

        public ReplayProtectionMiddleware(RequestDelegate next, IMemoryCache nonceCache)
        {
            _next = next;
            _nonceCache = nonceCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headers = context.Request.Headers;

            if (!headers.TryGetValue("X-Timestamp", out var timestampStr) ||
                !headers.TryGetValue("X-Nonce", out var nonce))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing timestamp or nonce");
                return;
            }

            if (!long.TryParse(timestampStr, out var timestamp))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid timestamp format");
                return;
            }

            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var skew = 30_000; // 30 seconds tolerance

            if (Math.Abs(now - timestamp) > skew)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Timestamp outside allowed window");
                return;
            }

            // Check nonce cache
            if (_nonceCache.TryGetValue(nonce, out _))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Replay detected");
                return;
            }

            // Store nonce with expiry
            _nonceCache.Set(nonce, true, TimeSpan.FromSeconds(30));

            await _next(context);
        }
    }
}
