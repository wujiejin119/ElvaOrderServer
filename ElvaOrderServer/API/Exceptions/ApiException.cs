using ElvaOrderServer.Domain.Constants;
using ElvaOrderServer.Domain.Exceptions;

namespace ElvaOrderServer.API.Exceptions
{
    public class ApiException : CustomException
    {
        public ApiException(
            string message,
            string errorType = ErrorTypes.API,
            Exception? innerException = null)
            : base(
                message: message,
                errorType: errorType,
                innerException: innerException)
        {
        }
    }
}
