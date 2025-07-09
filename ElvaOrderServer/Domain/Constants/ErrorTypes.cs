namespace ElvaOrderServer.Domain.Constants
{
    public static class ErrorTypes
    {
        // General error types
        public const string General = "general-error";
        public const string Domain = "domain-error";
        public const string Repository = "repository-error";
        public const string Application = "application-error";
        public const string Infrastructure = "infrastructure -error";
        public const string API = "api-error";

        // Specific error types, which should bring a specific error type to client to help distinguish between different errors.
        public const string InvalidParameter = "invalid-parameter";
        public const string NotFound = "not-found";
        public const string DataBaseError = "database-error";
    }
}