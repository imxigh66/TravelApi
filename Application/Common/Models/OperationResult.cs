using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class OperationResult<T>
    {
        public bool IsSuccess { get; init; }
        public string? Error { get; init; }
        public T? Data { get; init; }

        public static OperationResult<T> Success(T data) =>
            new() { IsSuccess = true, Data = data };

        public static OperationResult<T> Failure(string error) =>
            new() { IsSuccess = false, Error = error };
    }

}
