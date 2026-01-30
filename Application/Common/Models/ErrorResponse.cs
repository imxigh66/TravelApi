using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public string? StackTrace { get; set; }
        public DateTime Timestamp { get; set; }

        public ErrorResponse(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
            Timestamp = DateTime.UtcNow;
        }

        public ErrorResponse(string message, int statusCode, Dictionary<string, string[]> errors)
        {
            Message = message;
            StatusCode = statusCode;
            Errors = errors;
            Timestamp = DateTime.UtcNow;
        }

        // Статические методы для удобства
        public static ErrorResponse NotFound(string message = "Resource not found")
        {
            return new ErrorResponse(message, 404);
        }

        public static ErrorResponse BadRequest(string message = "Bad request")
        {
            return new ErrorResponse(message, 400);
        }

        public static ErrorResponse Unauthorized(string message = "Unauthorized")
        {
            return new ErrorResponse(message, 401);
        }

        public static ErrorResponse Forbidden(string message = "Forbidden")
        {
            return new ErrorResponse(message, 403);
        }

        public static ErrorResponse ValidationError(Dictionary<string, string[]> errors)
        {
            return new ErrorResponse(
                "One or more validation errors occurred",
                400,
                errors
            );
        }

        public static ErrorResponse InternalServerError(string message = "Internal server error")
        {
            return new ErrorResponse(message, 500);
        }
    }
}
