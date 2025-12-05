using System.Security.Claims;
using Flowertrack.Application.Dashboard.Queries.GetOrganizationDashboard;
using Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;
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
