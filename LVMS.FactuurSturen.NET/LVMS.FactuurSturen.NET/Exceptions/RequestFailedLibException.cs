using System;
using System.Net;

namespace LVMS.FactuurSturen.Exceptions
{
    public class RequestFailedLibException : FactuurSturenLibException
    {
        public HttpStatusCode StatusCode;

        public RequestFailedLibException(HttpStatusCode statusCode) : base()
        {
            StatusCode = statusCode;
        }

        public RequestFailedLibException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }

    public class RateLimitExceededLibException : RequestFailedLibException
    {
        public HttpStatusCode StatusCode;

        public RateLimitExceededLibException(HttpStatusCode statusCode) : base(statusCode)
        {
            
        }

        public RateLimitExceededLibException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
