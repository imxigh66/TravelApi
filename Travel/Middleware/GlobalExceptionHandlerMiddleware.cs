using Application.Common.Models;
using FluentValidation;
using System.Text.Json;

namespace TravelApi.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            ErrorResponse response;

            switch (exception)
            {
                case ValidationException validationEx:
                    response = CreateValidationErrorResponse(validationEx);
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;

                case UnauthorizedAccessException:
                    response = new ErrorResponse("Unauthorized", StatusCodes.Status401Unauthorized);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    break;

                case KeyNotFoundException:
                    response = new ErrorResponse("Resource not found", StatusCodes.Status404NotFound);
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;

                default:
                    response = CreateInternalServerErrorResponse(exception);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }

        private ErrorResponse CreateValidationErrorResponse(ValidationException ex)
        {
            // FluentValidation.ValidationException имеет свойство Errors типа IEnumerable<ValidationFailure>
            var errors = new Dictionary<string, string[]>();

            if (ex.Errors != null && ex.Errors.Any())
            {
                errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
            }

            return new ErrorResponse(
                "One or more validation errors occurred",
                StatusCodes.Status400BadRequest,
                errors
            );
        }

        private ErrorResponse CreateInternalServerErrorResponse(Exception ex)
        {
            var message = _env.IsDevelopment()
                ? ex.Message
                : "An internal server error occurred";

            var response = new ErrorResponse(message, StatusCodes.Status500InternalServerError);

            if (_env.IsDevelopment())
            {
                response.StackTrace = ex.StackTrace;
            }

            return response;
        }
    }
}
