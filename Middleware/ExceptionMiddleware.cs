using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservation.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionMiddleware>
            _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next =
                next;

            _logger =
                logger;
        }

        public async Task InvokeAsync(
            HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Unhandled exception occurred for {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await HandleExceptionAsync(
                    context,
                    exception);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            int statusCode =
                exception switch
                {
                    KeyNotFoundException =>
                        StatusCodes.Status404NotFound,

                    UnauthorizedAccessException =>
                        StatusCodes.Status401Unauthorized,

                    ArgumentException =>
                        StatusCodes.Status400BadRequest,

                    InvalidOperationException =>
                        StatusCodes.Status409Conflict,

                    DbUpdateException =>
                        StatusCodes.Status409Conflict,

                    _ =>
                        StatusCodes
                            .Status500InternalServerError
                };

            string message =
                statusCode ==
                    StatusCodes.Status500InternalServerError
                    ? "An unexpected server error occurred."
                    : exception.Message;

            context.Response.StatusCode =
                statusCode;

            context.Response.ContentType =
                "application/json";

            var response =
                new
                {
                    success =
                        false,

                    statusCode,

                    message
                };

            string json =
                JsonSerializer.Serialize(
                    response,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy =
                            JsonNamingPolicy.CamelCase
                    });

            await context.Response
                .WriteAsync(json);
        }
    }
}