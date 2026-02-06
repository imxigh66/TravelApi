using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiResponse(T data, string? message = null)
        {
            Success = true;
            Data = data;
            Message = message;
            Timestamp = DateTime.UtcNow;
        }

        public ApiResponse(string message)
        {
            Success = true;
            Message = message;
            Timestamp = DateTime.UtcNow;
        }

        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new ApiResponse<T>(data, message);
        }

        public static ApiResponse<object> SuccessResponse(string message)
        {
            return new ApiResponse<object>(message);
        }
    }
}
