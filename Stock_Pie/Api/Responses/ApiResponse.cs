namespace Stock_Pie.Api.Responses
{
    public sealed class ApiResponse<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }
        public object? Errors { get; init; }

        public static ApiResponse<T> ForSuccess(T? data, string? message = null) => new()
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null
        };

        public static ApiResponse<T> ForFailure(object? errors = null, string? message = null) => new()
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors
        };
    }
}