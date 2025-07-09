// Middleware/ExceptionHandlingMiddleware.cs
using ElvaOrderServer.Application.DTOs;
using ElvaOrderServer.Domain.Constants;
using ElvaOrderServer.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace ElvaOrderServer.API.Exceptions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly string _errorBaseUrl;

        private static readonly Dictionary<string, HttpStatusCode> _errorTypeStatusCodeMap = new()
        {
            { ErrorTypes.General, HttpStatusCode.InternalServerError },
            { ErrorTypes.Domain, HttpStatusCode.BadRequest },
            { ErrorTypes.Repository, HttpStatusCode.InternalServerError },
            { ErrorTypes.Application, HttpStatusCode.InternalServerError },
            { ErrorTypes.API, HttpStatusCode.InternalServerError },
            { ErrorTypes.InvalidParameter, HttpStatusCode.BadRequest },
            { ErrorTypes.NotFound, HttpStatusCode.NotFound }
        };


        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _errorBaseUrl = configuration["ErrorHandling:BaseUrl"]
                            ?? "https://errors.orderservcie.com/";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            var response = CreateProblemDetailsResponse(context, exception);
            context.Response.StatusCode = response.Status;

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }

        private ProblemDetailsResponse CreateProblemDetailsResponse(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;
            switch (exception)
            {
                case CustomException customEx:
                    _logger.LogError(customEx, "CustomException: {ErrorType}", customEx.ErrorType);
                    return new ProblemDetailsResponse
                    {
                        Type = $"{_errorBaseUrl}{customEx.ErrorType}",
                        Title = customEx.Message,
                        Status = (int)_errorTypeStatusCodeMap.GetValueOrDefault(customEx.ErrorType,
                                                         HttpStatusCode.InternalServerError),
                        TraceId = traceId
                    };


                case ArgumentException ex:
                    _logger.LogWarning(ex, "Invalid argument: {Message}", ex.Message);
                    return new ProblemDetailsResponse
                    {
                        Type = $"{_errorBaseUrl}invalid-argument",
                        Title = ex.Message,
                        Status = (int)HttpStatusCode.BadRequest,
                        Errors = new Dictionary<string, string[]>
                        {
                            ["Parameter"] = new[] { ex.ParamName ?? "Unknown" },
                            ["Details"] = new[] { ex.Message }
                        },
                        TraceId = traceId
                    };

                case DbUpdateException ex:
                    _logger.LogError(ex, "Database update error");
                    return new ProblemDetailsResponse
                    {
                        Type = $"{_errorBaseUrl}database-error",
                        Title = "Database operation failed",
                        Status = (int)HttpStatusCode.InternalServerError,
                        Errors = new Dictionary<string, string[]>
                        {
                            ["Details"] = new[] { ex.InnerException?.Message ?? ex.Message }
                        },
                        TraceId = traceId
                    };

                case UnauthorizedAccessException ex:
                    _logger.LogWarning(ex, "Unauthorized access");
                    return new ProblemDetailsResponse
                    {
                        Type = $"{_errorBaseUrl}unauthorized",
                        Title = "Access denied",
                        Status = (int)HttpStatusCode.Unauthorized,
                        TraceId = traceId
                    };

                default:
                    _logger.LogError(exception, "Unhandled exception");
                    return new ProblemDetailsResponse
                    {
                        Type = $"{_errorBaseUrl}internal-server-error",
                        Title = "An unexpected error occurred",
                        Status = (int)HttpStatusCode.InternalServerError,
                        Errors = new Dictionary<string, string[]>
                        {
                            ["Details"] = new[] { exception.Message }
                        },
                        TraceId = traceId
                    };
            }
        }
    }

}