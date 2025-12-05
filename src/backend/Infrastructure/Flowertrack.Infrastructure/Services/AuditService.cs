using System.Security.Claims;
using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Infrastructure.Services;

/// <summary>
/// Service for creating and persisting audit log entries.
/// Automatically captures user context from HTTP request.
/// </summary>
public sealed class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuditService> logger)
    {
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAsync(
        string actionType,
        string? resourceType = null,
        string? resourceId = null,
        string? description = null,
        string? oldValue = null,
        string? newValue = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entry = CreateEntry(actionType, resourceType, resourceId, description, oldValue, newValue);
            await _auditLogRepository.AddAsync(entry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Don't let audit logging failures break the main flow
            _logger.LogError(ex, "Failed to create audit log entry for action {ActionType}", actionType);
        }
    }

    public async Task LogFailureAsync(
        string actionType,
        string errorMessage,
        string? resourceType = null,
        string? resourceId = null,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entry = CreateEntry(
                actionType,
                resourceType,
                resourceId,
                description,
                oldValue: null,
                newValue: null,
                isSuccess: false,
                errorMessage);

            await _auditLogRepository.AddAsync(entry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create failure audit log entry for action {ActionType}", actionType);
        }
    }

    public async Task LogLoginAsync(
        string email,
        bool isSuccess,
        string? errorMessage = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (userId, _) = GetCurrentUserInfo();
            var httpContext = GetHttpContextInfo();

            var actionType = isSuccess ? AuditActionTypes.Login : AuditActionTypes.LoginFailed;
            var description = isSuccess
                ? $"User {email} logged in successfully"
                : $"Failed login attempt for {email}";

            var entry = AuditLog.Create(
                userId,
                email,
                actionType,
                resourceType: "User",
                resourceId: userId?.ToString(),
                description: description,
                oldValue: null,
                newValue: null,
                ipAddress: httpContext.IpAddress,
                userAgent: httpContext.UserAgent,
                httpMethod: httpContext.HttpMethod,
                requestPath: httpContext.RequestPath,
                correlationId: httpContext.CorrelationId,
                isSuccess: isSuccess,
                errorMessage: errorMessage);

            await _auditLogRepository.AddAsync(entry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create login audit log entry for {Email}", email);
        }
    }

    public async Task LogLogoutAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var (userId, userEmail) = GetCurrentUserInfo();
            var httpContext = GetHttpContextInfo();

            var entry = AuditLog.Create(
                userId,
                userEmail,
                AuditActionTypes.Logout,
                resourceType: "User",
                resourceId: userId?.ToString(),
                description: $"User {userEmail ?? "unknown"} logged out",
                ipAddress: httpContext.IpAddress,
                userAgent: httpContext.UserAgent,
                httpMethod: httpContext.HttpMethod,
                requestPath: httpContext.RequestPath,
                correlationId: httpContext.CorrelationId);

            await _auditLogRepository.AddAsync(entry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create logout audit log entry");
        }
    }

    public AuditLog CreateEntry(
        string actionType,
        string? resourceType = null,
        string? resourceId = null,
        string? description = null,
        string? oldValue = null,
        string? newValue = null,
        bool isSuccess = true,
        string? errorMessage = null)
    {
        var (userId, userEmail) = GetCurrentUserInfo();
        var httpContext = GetHttpContextInfo();

        var entry = AuditLog.Create(
            userId,
            userEmail,
            actionType,
            resourceType,
            resourceId,
            description,
            oldValue,
            newValue,
            httpContext.IpAddress,
            httpContext.UserAgent,
            httpContext.HttpMethod,
            httpContext.RequestPath,
            httpContext.CorrelationId,
            isSuccess,
            errorMessage);

        return entry;
    }

    private (Guid? UserId, string? UserEmail) GetCurrentUserInfo()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User == null)
        {
            return (null, null);
        }

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? httpContext.User.FindFirst("sub")?.Value;

        var userId = Guid.TryParse(userIdClaim, out var id) ? id : (Guid?)null;
        var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value
            ?? httpContext.User.FindFirst("email")?.Value;

        return (userId, userEmail);
    }

    private HttpContextInfo GetHttpContextInfo()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return new HttpContextInfo();
        }

        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        var httpMethod = httpContext.Request.Method;
        var requestPath = httpContext.Request.Path.ToString();
        var correlationId = httpContext.TraceIdentifier;

        return new HttpContextInfo
        {
            IpAddress = ipAddress,
            UserAgent = userAgent,
            HttpMethod = httpMethod,
            RequestPath = requestPath,
            CorrelationId = correlationId
        };
    }

    private sealed class HttpContextInfo
    {
        public string? IpAddress { get; init; }
        public string? UserAgent { get; init; }
        public string? HttpMethod { get; init; }
        public string? RequestPath { get; init; }
        public string? CorrelationId { get; init; }
    }
}
