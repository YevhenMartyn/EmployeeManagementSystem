
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BusinessLogicLayer.Exceptions
{
    public class CustomException : Exception
    {
        public int StatusCode { get; private set; }

        public CustomException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
