// Middleware/ExceptionHandlingMiddleware.cs
using ElvaOrderServer.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElvaOrderServer.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
            context.Response.ContentType = "application/json";
           
            var response = CreateExceptionResponse(exception);
            context.Response.StatusCode = response.StatusCode;

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }

        private ExceptionResponse CreateExceptionResponse(Exception exception)
        {
            switch (exception)
            {
                case OrderNotFoundException ex:
                    _logger.LogWarning(ex, "Order not found: {OrderId}", ex.OrderId);
                    return new ExceptionResponse(
                        statusCode: HttpStatusCode.NotFound,
                        errorType: "OrderNotFound",
                        message: ex.Message,
                        resourceId: ex.OrderId);

                case ArgumentException ex:
                    _logger.LogWarning(ex, "Invalid argument: {Message}", ex.Message);
                    return new ExceptionResponse(
                        statusCode: HttpStatusCode.BadRequest,
                        errorType: "InvalidArgument",
                        message: ex.Message,
                        details: ex.ParamName);

                case DbUpdateException ex:
                    _logger.LogError(ex, "Database update error");
                    return new ExceptionResponse(
                        statusCode: HttpStatusCode.InternalServerError,
                        errorType: "DatabaseError",
                        message: "Database operation failed",
                        details: ex.InnerException?.Message);

                case UnauthorizedAccessException ex:
                    _logger.LogWarning(ex, "Unauthorized access");
                    return new ExceptionResponse(
                        statusCode: HttpStatusCode.Unauthorized,
                        errorType: "Unauthorized",
                        message: "Access denied");

                default:
                    _logger.LogError(exception, "Unhandled exception");
                    return new ExceptionResponse(
                        statusCode: HttpStatusCode.InternalServerError,
                        errorType: "InternalServerError",
                        message: "An unexpected error occurred",
                        details: exception.Message);
            }
        }
    }

    public class ExceptionResponse
    {
        public int StatusCode { get; }

        public string ErrorType { get; }


        public string Message { get; }

        public Guid? ResourceId { get; }

        public DateTime Timestamp { get; } = DateTime.UtcNow;


        public string? Details { get; }

        public ExceptionResponse(
            HttpStatusCode statusCode,
            string errorType,
            string message,
            Guid? resourceId = null,
            string? details = null)
        {
            StatusCode = (int)statusCode;
            ErrorType = errorType;
            Message = message;
            ResourceId = resourceId;
            Details = details;
        }
    }
}