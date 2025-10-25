using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Flowertrack.Api.Extensions;

/// <summary>
/// Custom health check response writer that provides detailed information
/// </summary>
public static class HealthCheckResponseWriter
{
    /// <summary>
    /// Writes a detailed health check response
    /// </summary>
    public static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var result = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.TotalMilliseconds,
                exception = entry.Value.Exception?.Message,
                data = entry.Value.Data
            })
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(result, options));
    }
}
