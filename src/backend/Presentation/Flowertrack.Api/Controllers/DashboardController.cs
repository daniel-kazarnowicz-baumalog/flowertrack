using System.Security.Claims;
using Flowertrack.Application.Dashboard.Queries.GetOrganizationActivities;
using Flowertrack.Application.Dashboard.Queries.GetOrganizationDashboard;
using Flowertrack.Application.Dashboard.Queries.GetRecentActivities;
using Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;
using Flowertrack.Application.Dashboard.Queries.GetTicketTrends;
using Flowertrack.Application.Dashboard.Queries.GetUpcomingMaintenances;
using Flowertrack.Contracts.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowertrack.Api.Controllers;

/// <summary>
/// Controller for dashboard KPIs and metrics
/// US-006: Przeglądanie KPI serwisu
/// US-007: Przeglądanie KPI organizacji
/// US-037: Dashboard serwisu
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IMediator mediator,
        ILogger<DashboardController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get service dashboard with KPIs across all organizations.
    /// Available for ServiceAdministrator and ServiceTechnician users.
    /// </summary>
    [HttpGet("service")]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician")]
    [ProducesResponseType(typeof(ServiceDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetServiceDashboard()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse("Invalid user identity"));
        }

        _logger.LogInformation(
            "Service dashboard requested by user {UserId}",
            userId);

        var query = new GetServiceDashboardQuery
        {
            RequestedBy = userId
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Failed to get service dashboard: {Error}",
                result.Error);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponse(result.Error!));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get organization dashboard with KPIs for the user's organization.
    /// Available for OrganizationAdministrator and Operator users.
    /// </summary>
    [HttpGet("organization")]
    [Authorize(Roles = "OrganizationAdministrator,Operator")]
    [ProducesResponseType(typeof(OrganizationDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrganizationDashboard([FromQuery] Guid? organizationId = null)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse("Invalid user identity"));
        }

        _logger.LogInformation(
            "Organization dashboard requested by user {UserId} for org {OrgId}",
            userId,
            organizationId);

        var query = new GetOrganizationDashboardQuery
        {
            RequestedBy = userId,
            OrganizationId = organizationId
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(new ErrorResponse(result.Error));
            }

            _logger.LogWarning(
                "Failed to get organization dashboard: {Error}",
                result.Error);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponse(result.Error!));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get organization dashboard by ID.
    /// Service users can view any organization's dashboard.
    /// </summary>
    [HttpGet("organization/{organizationId:guid}")]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician")]
    [ProducesResponseType(typeof(OrganizationDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrganizationDashboardById(Guid organizationId)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse("Invalid user identity"));
        }

        _logger.LogInformation(
            "Organization dashboard for {OrgId} requested by service user {UserId}",
            organizationId,
            userId);

        var query = new GetOrganizationDashboardQuery
        {
            RequestedBy = userId,
            OrganizationId = organizationId
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(new ErrorResponse(result.Error));
            }

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ErrorResponse(result.Error!));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get ticket trends for the last N days.
    /// US-007: Wykres trendów zgłoszeń
    /// </summary>
    /// <param name="days">Number of days to retrieve trends for (default 30, max 90)</param>
    /// <param name="organizationId">Optional organization ID to filter trends</param>
    [HttpGet("trends")]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician")]
    [ProducesResponseType(typeof(TicketTrendsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTicketTrends(
        [FromQuery] int days = 30,
        [FromQuery] Guid? organizationId = null)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse("Invalid user identity"));
        }

        _logger.LogInformation(
            "Ticket trends requested by user {UserId}, days: {Days}, org: {OrgId}",
            userId, days, organizationId);

        var query = new GetTicketTrendsQuery
        {
            Days = days,
            OrganizationId = organizationId,
            RequestedBy = userId
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to get ticket trends: {Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(result.Error!));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get recent activities across the system.
    /// US-008: Lista ostatnich zdarzeń
    /// </summary>
    /// <param name="limit">Maximum number of activities to return (default 10, max 100)</param>
    /// <param name="organizationId">Optional organization ID to filter activities</param>
    [HttpGet("recent-activities")]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician")]
    [ProducesResponseType(typeof(RecentActivitiesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecentActivities(
        [FromQuery] int limit = 10,
        [FromQuery] Guid? organizationId = null)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse("Invalid user identity"));
        }

        _logger.LogInformation(
            "Recent activities requested by user {UserId}, limit: {Limit}, org: {OrgId}",
            userId, limit, organizationId);

        var query = new GetRecentActivitiesQuery
        {
            Limit = limit,
            OrganizationId = organizationId,
            RequestedBy = userId
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to get recent activities: {Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(result.Error!));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get upcoming scheduled maintenances.
    /// US-030: Konfiguracja przeglądów technicznych
    /// </summary>
    /// <param name="daysAhead">Number of days ahead to look (default 30, max 365)</param>
    /// <param name="limit">Maximum number of maintenances to return (default 20, max 100)</param>
    /// <param name="organizationId">Optional organization ID to filter maintenances</param>
    [HttpGet("upcoming-maintenances")]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician")]
    [ProducesResponseType(typeof(UpcomingMaintenancesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUpcomingMaintenances(
        [FromQuery] int daysAhead = 30,
        [FromQuery] int limit = 20,
        [FromQuery] Guid? organizationId = null)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse("Invalid user identity"));
        }

        _logger.LogInformation(
            "Upcoming maintenances requested by user {UserId}, daysAhead: {DaysAhead}, limit: {Limit}, org: {OrgId}",
            userId, daysAhead, limit, organizationId);

        var query = new GetUpcomingMaintenancesQuery
        {
            DaysAhead = daysAhead,
            Limit = limit,
            OrganizationId = organizationId,
            RequestedBy = userId
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to get upcoming maintenances: {Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(result.Error!));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get recent activities for a specific organization.
    /// US-038: Ostatnie aktywności (Portal Klienta)
    /// </summary>
    /// <param name="organizationId">Organization ID to get activities for</param>
    /// <param name="limit">Maximum number of activities to return (default 10, max 100)</param>
    [HttpGet("organization/{organizationId:guid}/activities")]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician,OrganizationAdministrator,Operator")]
    [ProducesResponseType(typeof(OrganizationActivitiesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrganizationActivities(
        Guid organizationId,
        [FromQuery] int limit = 10)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse("Invalid user identity"));
        }

        _logger.LogInformation(
            "Organization activities requested for org {OrgId} by user {UserId}, limit: {Limit}",
            organizationId, userId, limit);

        var query = new GetOrganizationActivitiesQuery
        {
            OrganizationId = organizationId,
            Limit = limit,
            RequestedBy = userId
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(new ErrorResponse(result.Error));
            }

            _logger.LogWarning("Failed to get organization activities: {Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(result.Error!));
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
