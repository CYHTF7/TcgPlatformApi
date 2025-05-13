using System.Net;

namespace TcgPlatformApi.Exceptions
{
    public class AppException : Exception
    {
        public string UserMessage { get; }  
        public HttpStatusCode StatusCode { get; } 
        public string LogMessage { get; }     

        public AppException(
            string userMessage,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest,
            string logMessage = "error")
        {
            UserMessage = userMessage;
            StatusCode = statusCode;
            LogMessage = logMessage ?? userMessage; 
        }
    }
}
