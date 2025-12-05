namespace Flowertrack.Domain.Entities;

/// <summary>
/// Represents an audit log entry for tracking all user actions in the system.
/// Provides a complete audit trail for security and compliance (US-054).
/// </summary>
/// <remarks>
/// This entity does not inherit from AuditableEntity because:
/// 1. It should never be modified or deleted
/// 2. It tracks other entities' changes, not its own
/// 3. Uses long Id for high-volume writes performance
/// </remarks>
public sealed class AuditLog
{
    /// <summary>
    /// Auto-incrementing identifier for the audit log entry
    /// </summary>
    public long Id { get; private set; }

    /// <summary>
    /// ID of the user who performed the action (null for system actions)
    /// </summary>
    public Guid? UserId { get; private set; }

    /// <summary>
    /// Email or username of the user for historical reference
    /// (stored because user details may change)
    /// </summary>
    public string? UserEmail { get; private set; }

    /// <summary>
    /// Type of action performed (e.g., CREATE, UPDATE, DELETE, LOGIN, LOGOUT, VIEW)
    /// </summary>
    public string ActionType { get; private set; } = string.Empty;

    /// <summary>
    /// Type of resource affected (e.g., Ticket, Machine, Organization, User)
    /// </summary>
    public string? ResourceType { get; private set; }

    /// <summary>
    /// ID of the affected resource
    /// </summary>
    public string? ResourceId { get; private set; }

    /// <summary>
    /// Human-readable description of the action
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Previous state of the resource as JSON (for UPDATE/DELETE actions)
    /// </summary>
    public string? OldValue { get; private set; }

    /// <summary>
    /// New state of the resource as JSON (for CREATE/UPDATE actions)
    /// </summary>
    public string? NewValue { get; private set; }

    /// <summary>
    /// IP address of the client that made the request
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// User agent string from the client
    /// </summary>
    public string? UserAgent { get; private set; }

    /// <summary>
    /// HTTP method used (GET, POST, PUT, DELETE, etc.)
    /// </summary>
    public string? HttpMethod { get; private set; }

    /// <summary>
    /// Request path/endpoint
    /// </summary>
    public string? RequestPath { get; private set; }

    /// <summary>
    /// Correlation ID for tracking related requests
    /// </summary>
    public string? CorrelationId { get; private set; }

    /// <summary>
    /// When the action occurred
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Whether the action was successful
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Error message if the action failed
    /// </summary>
    public string? ErrorMessage { get; private set; }

    // Private constructor for EF Core
    private AuditLog()
    {
    }

    // Private constructor for domain logic
    private AuditLog(
        Guid? userId,
        string? userEmail,
        string actionType,
        string? resourceType,
        string? resourceId,
        string? description,
        string? oldValue,
        string? newValue,
        string? ipAddress,
        string? userAgent,
        string? httpMethod,
        string? requestPath,
        string? correlationId,
        bool isSuccess,
        string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(actionType))
        {
            throw new ArgumentException("Action type is required", nameof(actionType));
        }

        UserId = userId;
        UserEmail = userEmail?.Length > 255 ? userEmail[..255] : userEmail;
        ActionType = actionType.ToUpperInvariant();
        ResourceType = resourceType;
        ResourceId = resourceId;
        Description = description?.Length > 1000 ? description[..1000] : description;
        OldValue = oldValue;
        NewValue = newValue;
        IpAddress = ipAddress?.Length > 45 ? ipAddress[..45] : ipAddress; // IPv6 max length
        UserAgent = userAgent?.Length > 500 ? userAgent[..500] : userAgent;
        HttpMethod = httpMethod?.ToUpperInvariant();
        RequestPath = requestPath?.Length > 2048 ? requestPath[..2048] : requestPath;
        CorrelationId = correlationId;
        CreatedAt = DateTimeOffset.UtcNow;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage?.Length > 2000 ? errorMessage[..2000] : errorMessage;
    }

    /// <summary>
    /// Factory method to create a new audit log entry
    /// </summary>
    public static AuditLog Create(
        Guid? userId,
        string? userEmail,
        string actionType,
        string? resourceType = null,
        string? resourceId = null,
        string? description = null,
        string? oldValue = null,
        string? newValue = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? httpMethod = null,
        string? requestPath = null,
        string? correlationId = null,
        bool isSuccess = true,
        string? errorMessage = null)
    {
        return new AuditLog(
            userId,
            userEmail,
            actionType,
            resourceType,
            resourceId,
            description,
            oldValue,
            newValue,
            ipAddress,
            userAgent,
            httpMethod,
            requestPath,
            correlationId,
            isSuccess,
            errorMessage);
    }

    /// <summary>
    /// Factory method for successful actions
    /// </summary>
    public static AuditLog CreateSuccess(
        Guid? userId,
        string? userEmail,
        string actionType,
        string? resourceType = null,
        string? resourceId = null,
        string? description = null,
        string? oldValue = null,
        string? newValue = null)
    {
        return new AuditLog(
            userId,
            userEmail,
            actionType,
            resourceType,
            resourceId,
            description,
            oldValue,
            newValue,
            ipAddress: null,
            userAgent: null,
            httpMethod: null,
            requestPath: null,
            correlationId: null,
            isSuccess: true,
            errorMessage: null);
    }

    /// <summary>
    /// Factory method for failed actions
    /// </summary>
    public static AuditLog CreateFailure(
        Guid? userId,
        string? userEmail,
        string actionType,
        string errorMessage,
        string? resourceType = null,
        string? resourceId = null,
        string? description = null)
    {
        return new AuditLog(
            userId,
            userEmail,
            actionType,
            resourceType,
            resourceId,
            description,
            oldValue: null,
            newValue: null,
            ipAddress: null,
            userAgent: null,
            httpMethod: null,
            requestPath: null,
            correlationId: null,
            isSuccess: false,
            errorMessage);
    }

    /// <summary>
    /// Sets HTTP request context information
    /// </summary>
    public AuditLog WithHttpContext(
        string? ipAddress,
        string? userAgent,
        string? httpMethod,
        string? requestPath,
        string? correlationId = null)
    {
        IpAddress = ipAddress?.Length > 45 ? ipAddress[..45] : ipAddress;
        UserAgent = userAgent?.Length > 500 ? userAgent[..500] : userAgent;
        HttpMethod = httpMethod?.ToUpperInvariant();
        RequestPath = requestPath?.Length > 2048 ? requestPath[..2048] : requestPath;
        CorrelationId = correlationId;
        return this;
    }
}

/// <summary>
/// Common audit action types
/// </summary>
public static class AuditActionTypes
{
    // Authentication
    public const string Login = "LOGIN";
    public const string Logout = "LOGOUT";
    public const string LoginFailed = "LOGIN_FAILED";
    public const string PasswordChanged = "PASSWORD_CHANGED";
    public const string PasswordReset = "PASSWORD_RESET";

    // CRUD
    public const string Create = "CREATE";
    public const string Read = "READ";
    public const string Update = "UPDATE";
    public const string Delete = "DELETE";

    // Tickets
    public const string TicketCreated = "TICKET_CREATED";
    public const string TicketUpdated = "TICKET_UPDATED";
    public const string TicketStatusChanged = "TICKET_STATUS_CHANGED";
    public const string TicketAssigned = "TICKET_ASSIGNED";
    public const string TicketCommentAdded = "TICKET_COMMENT_ADDED";
    public const string TicketAttachmentUploaded = "TICKET_ATTACHMENT_UPLOADED";

    // Machines
    public const string MachineRegistered = "MACHINE_REGISTERED";
    public const string MachineUpdated = "MACHINE_UPDATED";
    public const string MachineTokenGenerated = "MACHINE_TOKEN_GENERATED";
    public const string MachineLogReceived = "MACHINE_LOG_RECEIVED";

    // Organizations
    public const string OrganizationCreated = "ORGANIZATION_CREATED";
    public const string OrganizationUpdated = "ORGANIZATION_UPDATED";
    public const string UserInvited = "USER_INVITED";
    public const string UserRemoved = "USER_REMOVED";

    // Admin
    public const string Export = "EXPORT";
    public const string BulkAction = "BULK_ACTION";
}
