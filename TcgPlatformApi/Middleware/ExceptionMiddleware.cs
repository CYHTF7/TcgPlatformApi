using System.Net;
using TcgPlatformApi.Exceptions;

namespace TcgPlatformApi.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            if (ex is AppException appEx)
            {
                _logger.LogError("Error: {LogMessage}", appEx.LogMessage); 
                context.Response.StatusCode = (int)appEx.StatusCode;
                await context.Response.WriteAsJsonAsync(new { error = appEx.UserMessage });
                return;
            }

            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "Server error" });
        }
    }
}
