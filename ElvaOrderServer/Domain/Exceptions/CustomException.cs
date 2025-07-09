using ElvaOrderServer.Domain.Constants;

namespace ElvaOrderServer.Domain.Exceptions
{
    public abstract class CustomException : Exception
    {
        public string ErrorType { get; }

        protected CustomException(
            string message,
            string errorType = ErrorTypes.General,
            Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorType = errorType;
        }
    }
}