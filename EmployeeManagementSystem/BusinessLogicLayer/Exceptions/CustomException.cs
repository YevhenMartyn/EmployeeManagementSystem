
using Microsoft.AspNetCore.Http;
using System.Net;

namespace BusinessLogicLayer.Exceptions
{
    public class CustomException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public CustomException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
