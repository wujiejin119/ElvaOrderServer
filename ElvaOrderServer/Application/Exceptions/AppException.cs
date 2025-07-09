using ElvaOrderServer.Domain.Constants;

namespace ElvaOrderServer.Domain.Exceptions
{
    public class AppException : CustomException
    {
        public AppException(
            string message,
            string errorType = ErrorTypes.Application,
            Exception? innerException = null)
            : base(
                message: message,
                errorType: errorType,
                innerException: innerException)
        {
        }
    }
}