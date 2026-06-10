using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Stock_Pie.Api.Responses;
using System.Text.Json;

namespace Stock_Pie.Middleware
{
    public class GlobalExceptionMiddleWare(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionMiddleWare> logger
    ) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception occurred.");

            var (statusCode, title) = GetExceptionDetails(exception);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = IsDevelopment() ? exception.Message : "An unexpected error occurred.",
                Instance = httpContext.Request.Path,
                Type = $"https://httpstatuses.com/{statusCode}"
            };

            // Try write using standard problem details pipeline
            var wrote = await problemDetailsService.TryWriteAsync(new()
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails
            });

            if (wrote) return true;

            // Fallback: write our ApiResponse wrapper
            var apiError = ApiResponse<object>.ForFailure(new
            {
                title = problemDetails.Title,
                detail = problemDetails.Detail,
                instance = problemDetails.Instance
            }, message: problemDetails.Title);

            var json = JsonSerializer.Serialize(apiError, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsync(json);

            return true;
        }

        private static (int StatusCode, string Title) GetExceptionDetails(Exception ex) =>
            ex switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
                UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Access denied"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid operation"),

                _ => (StatusCodes.Status500InternalServerError, "Internal server error")
            };

        private static bool IsDevelopment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        }
    }
}