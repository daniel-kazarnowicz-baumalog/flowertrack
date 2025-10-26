namespace Flowertrack.Api.Infrastructure;

/// <summary>
/// Extension methods for structured logging
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Logs ticket creation with structured properties
    /// </summary>
    public static void LogTicketCreated(this ILogger logger, int ticketId, string title, string createdBy)
    {
        logger.LogInformation(
            "Ticket created: {TicketId} - {Title} by {CreatedBy}",
            ticketId, title, createdBy);
    }

    /// <summary>
    /// Logs ticket status change with structured properties
    /// </summary>
    public static void LogTicketStatusChanged(this ILogger logger, int ticketId, string oldStatus, string newStatus, string changedBy)
    {
        logger.LogInformation(
            "Ticket status changed: {TicketId} from {OldStatus} to {NewStatus} by {ChangedBy}",
            ticketId, oldStatus, newStatus, changedBy);
    }

    /// <summary>
    /// Logs ticket assignment with structured properties
    /// </summary>
    public static void LogTicketAssigned(this ILogger logger, int ticketId, string assignedTo, string assignedBy)
    {
        logger.LogInformation(
            "Ticket assigned: {TicketId} to {AssignedTo} by {AssignedBy}",
            ticketId, assignedTo, assignedBy);
    }

    /// <summary>
    /// Logs user action with structured properties
    /// </summary>
    public static void LogUserAction(this ILogger logger, string action, string userId, string? resourceType = null, string? resourceId = null)
    {
        if (resourceType != null && resourceId != null)
        {
            logger.LogInformation(
                "User action: {Action} by {UserId} on {ResourceType} {ResourceId}",
                action, userId, resourceType, resourceId);
        }
        else
        {
            logger.LogInformation(
                "User action: {Action} by {UserId}",
                action, userId);
        }
    }

    /// <summary>
    /// Logs authentication events with structured properties
    /// </summary>
    public static void LogAuthenticationEvent(this ILogger logger, string eventType, string userId, bool success, string? reason = null)
    {
        if (success)
        {
            logger.LogInformation(
                "Authentication event: {EventType} for user {UserId} - Success",
                eventType, userId);
        }
        else
        {
            logger.LogWarning(
                "Authentication event: {EventType} for user {UserId} - Failed. Reason: {Reason}",
                eventType, userId, reason ?? "Unknown");
        }
    }

    /// <summary>
    /// Logs service errors with structured properties
    /// </summary>
    public static void LogServiceError(this ILogger logger, Exception exception, string serviceName, string operation, Dictionary<string, object>? additionalData = null)
    {
        if (additionalData != null && additionalData.Any())
        {
            logger.LogError(
                exception,
                "Service error in {ServiceName}.{Operation}: {ErrorMessage}. Additional data: {@AdditionalData}",
                serviceName, operation, exception.Message, additionalData);
        }
        else
        {
            logger.LogError(
                exception,
                "Service error in {ServiceName}.{Operation}: {ErrorMessage}",
                serviceName, operation, exception.Message);
        }
    }

    /// <summary>
    /// Logs database operations with structured properties
    /// </summary>
    public static void LogDatabaseOperation(this ILogger logger, string operation, string entityType, object? entityId = null, long? durationMs = null)
    {
        if (entityId != null && durationMs.HasValue)
        {
            logger.LogInformation(
                "Database operation: {Operation} on {EntityType} {EntityId} completed in {DurationMs}ms",
                operation, entityType, entityId, durationMs.Value);
        }
        else if (entityId != null)
        {
            logger.LogInformation(
                "Database operation: {Operation} on {EntityType} {EntityId}",
                operation, entityType, entityId);
        }
        else
        {
            logger.LogInformation(
                "Database operation: {Operation} on {EntityType}",
                operation, entityType);
        }
    }

    /// <summary>
    /// Logs external API calls with structured properties
    /// </summary>
    public static void LogExternalApiCall(this ILogger logger, string apiName, string endpoint, string method, int statusCode, long durationMs)
    {
        if (statusCode >= 200 && statusCode < 300)
        {
            logger.LogInformation(
                "External API call: {Method} {ApiName}{Endpoint} returned {StatusCode} in {DurationMs}ms",
                method, apiName, endpoint, statusCode, durationMs);
        }
        else
        {
            logger.LogWarning(
                "External API call failed: {Method} {ApiName}{Endpoint} returned {StatusCode} in {DurationMs}ms",
                method, apiName, endpoint, statusCode, durationMs);
        }
    }
}
