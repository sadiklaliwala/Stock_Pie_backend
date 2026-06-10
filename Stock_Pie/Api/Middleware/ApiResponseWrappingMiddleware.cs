using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Stock_Pie.Api.Responses;

namespace Stock_Pie.Api.Middleware
{
    // Middleware to wrap controller responses in ApiResponse<T>
    public class ApiResponseWrappingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiResponseWrappingMiddleware> _logger;

        public ApiResponseWrappingMiddleware(RequestDelegate next, ILogger<ApiResponseWrappingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Keep original body
            var originalBody = context.Response.Body;

            await using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            try
            {
                await _next(context);

                // Rewind and read response
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var bodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                // If response is already an ApiResponse, just copy through
                if (IsApiResponse(bodyText))
                {
                    await memoryStream.CopyToAsync(originalBody);
                    return;
                }

                // Map status code to success boolean
                var success = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300;

                object? data = null;
                object? errors = null;

                if (!string.IsNullOrWhiteSpace(bodyText))
                {
                    try
                    {
                        data = JsonSerializer.Deserialize<object>(bodyText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize response body when wrapping");
                        // treat body as raw string
                        data = bodyText;
                    }
                }

                if (!success)
                {
                    // Try to extract problems from ProblemDetails or validation model
                    if (context.Response.ContentType?.Contains("application/problem+json") == true || IsProblemDetails(data))
                    {
                        errors = data ?? new { message = "An error occurred" };
                    }
                    else
                    {
                        errors = data;
                        data = null;
                    }
                }

                var wrapperType = typeof(ApiResponse<object>);
                var wrapper = new ApiResponse<object>
                {
                    Success = success,
                    Message = success ? null : "An error occurred",
                    Data = success ? data : default,
                    Errors = !success ? errors : null
                };

                context.Response.ContentType = "application/json";
                context.Response.Body = originalBody;

                // Preserve status code
                var json = JsonSerializer.Serialize(wrapper, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                context.Response.ContentLength = System.Text.Encoding.UTF8.GetByteCount(json);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ApiResponseWrappingMiddleware");
                context.Response.Body = originalBody;
                throw; // let global exception handler handle
            }
        }

        private bool IsApiResponse(string bodyText)
        {
            if (string.IsNullOrWhiteSpace(bodyText)) return false;
            try
            {
                using var doc = JsonDocument.Parse(bodyText);
                return doc.RootElement.TryGetProperty("success", out _);
            }
            catch
            {
                return false;
            }
        }

        private bool IsProblemDetails(object? obj)
        {
            if (obj is null) return false;
            try
            {
                var json = JsonSerializer.Serialize(obj);
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.TryGetProperty("title", out _) && doc.RootElement.TryGetProperty("status", out _);
            }
            catch
            {
                return false;
            }
        }
    }
}