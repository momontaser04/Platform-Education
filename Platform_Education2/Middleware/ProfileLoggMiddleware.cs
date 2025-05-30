using System.Diagnostics;

namespace PlatformEduPro.Middleware
{
    public class ProfileLoggMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ProfileLoggMiddleware> _logger;

        public ProfileLoggMiddleware(RequestDelegate next, ILogger<ProfileLoggMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"[Request] {context.Request.Method} {context.Request.Path}");

            try
            {
                await _next(context);
                stopwatch.Stop();
                _logger.LogInformation($"[Response] {context.Response.StatusCode} ({stopwatch.ElapsedMilliseconds} ms)");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"[Error] حدث خطأ أثناء معالجة الطلب: {context.Request.Path}");
                throw;
            }
        }
    }
}
