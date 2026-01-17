using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class OperationResult<T>
    {
        public bool IsSuccess { get; init; }
        public string? Error { get; init; }
        public T? Data { get; init; }
    }

}
