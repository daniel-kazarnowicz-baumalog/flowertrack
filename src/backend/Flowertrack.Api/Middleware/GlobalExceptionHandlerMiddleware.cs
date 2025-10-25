using System.Net;
using System.Text.Json;
using Flowertrack.Api.Contracts.Common;
using Flowertrack.Api.Exceptions;

namespace Flowertrack.Api.Middleware;

/// <summary>
/// Global exception handler middleware that catches all unhandled exceptions
/// and returns RFC 7807 compliant error responses
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
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
        var (statusCode, type, title) = GetErrorDetails(exception);

        _logger.LogError(exception, "An error occurred: {ErrorType} - {ErrorMessage}", 
            exception.GetType().Name, exception.Message);

        var response = new ErrorResponse
        {
            Type = type,
            Title = title,
            Status = (int)statusCode,
            Detail = _environment.IsDevelopment() ? exception.Message : GetUserFriendlyMessage(exception),
            Instance = context.Request.Path,
            Extensions = GetErrorExtensions(exception)
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }

    private static (HttpStatusCode statusCode, string type, string title) GetErrorDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException => (
                HttpStatusCode.BadRequest,
                "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                "Validation Error"
            ),
            NotFoundException => (
                HttpStatusCode.NotFound,
                "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                "Resource Not Found"
            ),
            UnauthorizedException => (
                HttpStatusCode.Unauthorized,
                "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                "Unauthorized"
            ),
            ForbiddenException => (
                HttpStatusCode.Forbidden,
                "https://tools.ietf.org/html/rfc9110#section-15.5.4",
                "Forbidden"
            ),
            DomainException => (
                HttpStatusCode.BadRequest,
                "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                "Business Rule Violation"
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                "Internal Server Error"
            )
        };
    }

    private static string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            ValidationException => "One or more validation errors occurred",
            NotFoundException => "The requested resource was not found",
            UnauthorizedException => "Authentication is required to access this resource",
            ForbiddenException => "You do not have permission to access this resource",
            DomainException => "A business rule was violated",
            _ => "An unexpected error occurred. Please try again later"
        };
    }

    private Dictionary<string, object>? GetErrorExtensions(Exception exception)
    {
        if (exception is ValidationException validationException && validationException.Errors.Any())
        {
            return new Dictionary<string, object>
            {
                { "errors", validationException.Errors }
            };
        }

        // In development, include stack trace
        if (_environment.IsDevelopment())
        {
            return new Dictionary<string, object>
            {
                { "stackTrace", exception.StackTrace ?? string.Empty },
                { "exceptionType", exception.GetType().FullName ?? string.Empty }
            };
        }

        return null;
    }
}
