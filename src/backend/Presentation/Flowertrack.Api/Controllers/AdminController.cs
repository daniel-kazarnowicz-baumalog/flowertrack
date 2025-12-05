using System.Security.Claims;
using Flowertrack.Application.Audit.Queries.GetAuditLogs;
using Flowertrack.Application.Common.Models;
using Flowertrack.Contracts.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowertrack.Api.Controllers;

/// <summary>
/// Controller for administrative functions including audit logs.
/// US-054: Pełny audyt akcji użytkowników
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "ServiceAdministrator")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IMediator mediator,
        ILogger<AdminController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get audit logs with optional filters.
    /// Only ServiceAdministrator users can access audit logs.
    /// </summary>
    [HttpGet("audit-logs")]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] Guid? userId = null,
        [FromQuery] string? actionType = null,
        [FromQuery] string? resourceType = null,
        [FromQuery] string? resourceId = null,
        [FromQuery] DateTimeOffset? fromDate = null,
        [FromQuery] DateTimeOffset? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var requestedBy = GetCurrentUserId();
        if (requestedBy == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse("Invalid user identity"));
        }

        _logger.LogInformation(
            "Audit logs requested by admin {UserId}, filters: User={FilterUserId}, Action={ActionType}, Resource={ResourceType}",
            requestedBy,
            userId,
            actionType,
            resourceType);

        var query = new GetAuditLogsQuery
        {
            RequestedBy = requestedBy,
            UserId = userId,
            ActionType = actionType,
            ResourceType = resourceType,
            ResourceId = resourceId,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to retrieve audit logs: {Error}", result.Error);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponse(result.Error!));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get audit logs for a specific user.
    /// </summary>
    [HttpGet("audit-logs/user/{userId:guid}")]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAuditLogsByUser(
        Guid userId,
        [FromQuery] DateTimeOffset? fromDate = null,
        [FromQuery] DateTimeOffset? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var requestedBy = GetCurrentUserId();
        if (requestedBy == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse("Invalid user identity"));
        }

        _logger.LogInformation(
            "Audit logs for user {TargetUserId} requested by admin {AdminUserId}",
            userId,
            requestedBy);

        var query = new GetAuditLogsQuery
        {
            RequestedBy = requestedBy,
            UserId = userId,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponse(result.Error!));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get audit logs for a specific resource.
    /// </summary>
    [HttpGet("audit-logs/resource/{resourceType}/{resourceId}")]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAuditLogsByResource(
        string resourceType,
        string resourceId,
        [FromQuery] DateTimeOffset? fromDate = null,
        [FromQuery] DateTimeOffset? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var requestedBy = GetCurrentUserId();
        if (requestedBy == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse("Invalid user identity"));
        }

        _logger.LogInformation(
            "Audit logs for resource {ResourceType}/{ResourceId} requested by admin {AdminUserId}",
            resourceType,
            resourceId,
            requestedBy);

        var query = new GetAuditLogsQuery
        {
            RequestedBy = requestedBy,
            ResourceType = resourceType,
            ResourceId = resourceId,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponse(result.Error!));
        }

        return Ok(result.Value);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Guid.Empty;
        }

        return userId;
    }
}
