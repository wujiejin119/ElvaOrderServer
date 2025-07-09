using ElvaOrderServer.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Net;

namespace ElvaOrderServer.Domain.Exceptions
{
    public class DomainException : CustomException
    {
        public DomainException(
            string message,
            string errorType = ErrorTypes.Domain,
            Exception? innerException = null)
            : base(
                message: message,
                errorType: errorType,
                innerException: innerException)
        {
        }
    }
}