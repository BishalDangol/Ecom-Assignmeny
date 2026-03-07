using System;

namespace Saja.BLL
{
    /// <summary>
    /// Represents the result of a business logic operation.
    /// </summary>
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public OperationResult(bool success, string message, object data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static OperationResult Succeeded(string message = "Operation successful", object data = null)
        {
            return new OperationResult(true, message, data);
        }

        public static OperationResult Failed(string message = "Operation failed")
        {
            return new OperationResult(false, message);
        }
    }
}
