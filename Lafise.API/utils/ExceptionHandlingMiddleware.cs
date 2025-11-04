using System.Net;
using System.Text.Json;

namespace Lafise.API.utils
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            ErrorResponseDto errorResponse;

            if (exception is LafiseException lafiseException)
            {
                response.StatusCode = lafiseException.Code;
                errorResponse = new ErrorResponseDto
                {
                    Error = true,
                    Code = lafiseException.Code,
                    Message = lafiseException.Message,
                    Detail = exception.StackTrace
                };
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = new ErrorResponseDto
                {
                    Error = true,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = exception.Message ?? "An error occurred while processing your request",
                    Detail = exception.StackTrace
                };
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return response.WriteAsync(jsonResponse);
        }
    }
}

