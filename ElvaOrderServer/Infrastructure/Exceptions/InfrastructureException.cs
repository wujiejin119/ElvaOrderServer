using ElvaOrderServer.Domain.Constants;

namespace ElvaOrderServer.Domain.Exceptions
{
    public class InfrastructureException : CustomException
    {
        public InfrastructureException(
            string message,
            string errorType = ErrorTypes.Infrastructure,
            Exception? innerException = null)
            : base(
                message: message,
                errorType: errorType,
                innerException: innerException)
        {
        }
    }
}