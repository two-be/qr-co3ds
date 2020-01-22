using System;
namespace QrCo3ds.Models
{
    public class ExceptionInfo
    {
        public string Message { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;

        public ExceptionInfo(string message) => Message = message;

        public ExceptionInfo(string message, string stackTrace) => (Message, StackTrace) = (message, stackTrace);
    }
}
