using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TekTrov.Application.Common
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; private set; }
        public string Message { get; private set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; private set; }

        public static ApiResponse<T> SuccessResponse(
            T? data,
            string message,
            int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Message = message,  
                Data = data
            };
        }

        public static ApiResponse<T> FailureResponse(
            string message,
            int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Message = message,
                Data = default
            };
        }
    }
}
