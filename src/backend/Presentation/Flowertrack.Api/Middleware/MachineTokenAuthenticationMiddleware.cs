using Flowertrack.Domain.Repositories;

namespace Flowertrack.Api.Middleware;

/// <summary>
/// Middleware that authenticates requests using machine API tokens.
/// This middleware is applied to specific endpoints (e.g., /api/ingest/logs)
/// and validates the X-API-Token header against registered machine tokens.
/// </summary>
public class MachineTokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MachineTokenAuthenticationMiddleware> _logger;

    /// <summary>
    /// Header name for the machine API token
    /// </summary>
    public const string ApiTokenHeader = "X-API-Token";

    /// <summary>
    /// Context item key for the authenticated machine ID
    /// </summary>
    public const string MachineIdContextKey = "AuthenticatedMachineId";

    public MachineTokenAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<MachineTokenAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IMachineRepository machineRepository)
    {
        // Only apply to paths that require machine token authentication
        if (!ShouldAuthenticateRequest(context.Request))
        {
            await _next(context);
            return;
        }

        // Check for API token header
        if (!context.Request.Headers.TryGetValue(ApiTokenHeader, out var tokenValues) ||
            string.IsNullOrWhiteSpace(tokenValues.FirstOrDefault()))
        {
            _logger.LogWarning(
                "Machine token authentication failed: Missing {HeaderName} header for path {Path}",
                ApiTokenHeader,
                context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                title = "Unauthorized",
                status = 401,
                detail = $"Missing or empty {ApiTokenHeader} header"
            });
            return;
        }

        var apiToken = tokenValues.First()!;

        // Validate token against database
        var machine = await machineRepository.GetByApiTokenAsync(apiToken, context.RequestAborted);

        if (machine == null)
        {
            _logger.LogWarning(
                "Machine token authentication failed: Invalid API token for path {Path}",
                context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                title = "Unauthorized",
                status = 401,
                detail = "Invalid API token"
            });
            return;
        }

        if (machine.IsDeleted)
        {
            _logger.LogWarning(
                "Machine token authentication failed: Machine {MachineId} is deleted",
                machine.Id);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                title = "Unauthorized",
                status = 401,
                detail = "Machine is no longer active"
            });
            return;
        }

        // Store the authenticated machine ID in context for use by controllers
        context.Items[MachineIdContextKey] = machine.Id;

        _logger.LogDebug(
            "Machine token authentication successful for machine {MachineId} ({SerialNumber})",
            machine.Id,
            machine.SerialNumber);

        await _next(context);
    }

    /// <summary>
    /// Determines if the request should be authenticated using machine token
    /// </summary>
    private static bool ShouldAuthenticateRequest(HttpRequest request)
    {
        // Apply to /api/ingest/* endpoints
        return request.Path.StartsWithSegments("/api/ingest", StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Extension methods for registering MachineTokenAuthenticationMiddleware
/// </summary>
public static class MachineTokenAuthenticationMiddlewareExtensions
{
    /// <summary>
    /// Adds the machine token authentication middleware to the application pipeline.
    /// Should be placed before authentication middleware for regular endpoints.
    /// </summary>
    public static IApplicationBuilder UseMachineTokenAuthentication(this IApplicationBuilder app)
    {
        return app.UseMiddleware<MachineTokenAuthenticationMiddleware>();
    }
}
