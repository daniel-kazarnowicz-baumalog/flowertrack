using Flowertrack.Application.Tickets.Commands.CreateTicket;
using Flowertrack.Application.Tickets.Commands.DeleteTicket;
using Flowertrack.Application.Tickets.Commands.UpdateTicket;
using Flowertrack.Application.Tickets.Commands.UpdateTicketStatus;
using Flowertrack.Application.Tickets.Queries.GetTicket;
using Flowertrack.Contracts.Common;
using Flowertrack.Contracts.Tickets.Requests;
using Flowertrack.Contracts.Tickets.Responses;
using Flowertrack.Contracts.Tickets;
using Flowertrack.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flowertrack.Api.Controllers;

/// <summary>
/// Controller for managing service tickets
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(
        IMediator mediator,
        ILogger<TicketsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new service ticket
    /// US-015: Tworzenie zgłoszenia serwisowego
    /// </summary>
    /// <param name="request">Ticket creation details</param>
    /// <returns>Created ticket details with location header</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTicketResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
    {
        // Get current user ID from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("User ID not found in claims or invalid format");
            return Unauthorized(new ErrorResponse("User authentication failed"));
        }

        var command = new CreateTicketCommand
        {
            OrganizationId = request.OrganizationId,
            MachineId = request.MachineId,
            Title = request.Title,
            Description = request.Description,
            Priority = (Priority)request.Priority,
            CreatedBy = userId
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Failed to create ticket for organization {OrganizationId}: {Error}",
                request.OrganizationId,
                result.Error);

            // Check if it's a validation/permission error (403) or general bad request (400)
            if (result.Error!.Contains("permission", StringComparison.OrdinalIgnoreCase) ||
                result.Error.Contains("forbidden", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ErrorResponse(result.Error));
            }

            return BadRequest(new ErrorResponse(result.Error));
        }

        var ticketId = result.Value;

        // In real implementation, we would query the created ticket to get full details
        // For now, we return minimal response
        var response = new CreateTicketResponse
        {
            Id = ticketId,
            TicketNumber = "TICK-TEMP", // TODO: Get actual ticket number from repository
            Title = request.Title,
            Status = TicketStatus.New.ToString(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _logger.LogInformation(
            "Successfully created ticket {TicketId} for organization {OrganizationId}",
            ticketId,
            request.OrganizationId);

        // Return 201 Created with Location header
        return CreatedAtAction(
            nameof(GetTicket),
            new { id = ticketId },
            response);
    }

    /// <summary>
    /// Get a specific ticket by ID
    /// US-015: Wyświetlanie szczegółów zgłoszenia
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <returns>Detailed ticket information</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicket(Guid id)
    {
        // Get current user ID from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("User ID not found in claims or invalid format");
            return Unauthorized(new ErrorResponse("User authentication failed"));
        }

        var query = new GetTicketQuery
        {
            TicketId = id,
            RequestedBy = userId
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to get ticket {TicketId}: {Error}", id, result.Error);

            if (result.Error!.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ErrorResponse(result.Error));
            }

            if (result.Error.Contains("permission", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ErrorResponse(result.Error));
            }

            return BadRequest(new ErrorResponse(result.Error));
        }

        var ticket = result.Value;

        // Map to response DTO
        var response = new TicketResponse
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            Priority = ticket.Priority,
            OrganizationId = ticket.OrganizationId,
            OrganizationName = ticket.OrganizationName,
            MachineId = ticket.MachineId,
            MachineSerialNumber = ticket.MachineSerialNumber,
            MachineModel = $"{ticket.MachineBrand} {ticket.MachineModel}",
            CreatedByUserId = ticket.CreatedByUserId,
            CreatedByUserName = ticket.CreatedByUserName,
            AssignedToUserId = ticket.AssignedToUserId,
            AssignedToUserName = ticket.AssignedToUserName,
            ResolvedAt = ticket.ResolvedAt,
            ClosedAt = ticket.ClosedAt,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            UpdatedBy = ticket.UpdatedBy
        };

        return Ok(response);
    }

    /// <summary>
    /// Update an existing ticket's basic information
    /// US-015: Edycja zgłoszenia serwisowego
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="request">Updated ticket information</param>
    /// <returns>Success indicator</returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateTicket(Guid id, [FromBody] UpdateTicketRequest request)
    {
        // Get current user ID from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("User ID not found in claims or invalid format");
            return Unauthorized(new ErrorResponse("User authentication failed"));
        }

        var command = new UpdateTicketCommand
        {
            TicketId = id,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority.HasValue ? (Priority)request.Priority.Value : null,
            UpdatedBy = userId
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Failed to update ticket {TicketId}: {Error}",
                id,
                result.Error);

            if (result.Error!.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ErrorResponse(result.Error));
            }

            if (result.Error.Contains("permission", StringComparison.OrdinalIgnoreCase) ||
                result.Error.Contains("forbidden", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ErrorResponse(result.Error));
            }

            if (result.Error.Contains("closed", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new ErrorResponse(result.Error));
            }

            return BadRequest(new ErrorResponse(result.Error));
        }

        _logger.LogInformation("Successfully updated ticket {TicketId}", id);

        return NoContent();
    }

    /// <summary>
    /// Soft delete a ticket
    /// US-015: Usuwanie zgłoszenia serwisowego
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="reason">Optional reason for deletion</param>
    /// <returns>Success indicator</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteTicket(Guid id, [FromQuery] string? reason = null)
    {
        // Get current user ID from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("User ID not found in claims or invalid format");
            return Unauthorized(new ErrorResponse("User authentication failed"));
        }

        var command = new DeleteTicketCommand
        {
            TicketId = id,
            Reason = reason,
            DeletedBy = userId
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Failed to delete ticket {TicketId}: {Error}",
                id,
                result.Error);

            if (result.Error!.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ErrorResponse(result.Error));
            }

            if (result.Error.Contains("permission", StringComparison.OrdinalIgnoreCase) ||
                result.Error.Contains("forbidden", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ErrorResponse(result.Error));
            }

            if (result.Error.Contains("cannot be deleted", StringComparison.OrdinalIgnoreCase) ||
                result.Error.Contains("already deleted", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new ErrorResponse(result.Error));
            }

            return BadRequest(new ErrorResponse(result.Error));
        }

        _logger.LogInformation("Successfully deleted ticket {TicketId}", id);

        return NoContent();
    }

    /// <summary>
    /// Update ticket status with state machine validation
    /// US-015: Zmiana statusu zgłoszenia serwisowego
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="request">Status update details</param>
    /// <returns>No content on success</returns>
    /// <remarks>
    /// Valid state transitions:
    /// - New → Accepted, Closed
    /// - Accepted → InProgress, Closed
    /// - InProgress → Resolved, Closed
    /// - Resolved → Closed, Reopened (within 14 days)
    /// - Reopened → InProgress, Resolved, Closed
    /// - Closed → (no transitions, final state)
    /// 
    /// Reason is required for Resolved and Closed transitions.
    /// </remarks>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateTicketStatus(
        [FromRoute] Guid id,
        [FromBody] UpdateTicketStatusRequest request)
    {
        // Get current user ID from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("User ID not found in claims or invalid format");
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        _logger.LogInformation(
            "Updating status for ticket {TicketId} to {Status}",
            id,
            (TicketStatus)request.Status);

        var command = new UpdateTicketStatusCommand(
            id,
            (TicketStatus)request.Status,
            request.Reason,
            userId);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Failed to update ticket {TicketId} status: {Error}",
                id,
                result.Error);

            if (result.Error!.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ErrorResponse(result.Error));
            }

            if (result.Error.Contains("permission", StringComparison.OrdinalIgnoreCase) ||
                result.Error.Contains("forbidden", StringComparison.OrdinalIgnoreCase) ||
                result.Error.Contains("not a member", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new ErrorResponse(result.Error));
            }

            if (result.Error.Contains("transition", StringComparison.OrdinalIgnoreCase) ||
                result.Error.Contains("conflict", StringComparison.OrdinalIgnoreCase) ||
                result.Error.Contains("Cannot transition", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new ErrorResponse(result.Error));
            }

            return BadRequest(new ErrorResponse(result.Error));
        }

        _logger.LogInformation(
            "Successfully updated status for ticket {TicketId} to {Status}",
            id,
            (TicketStatus)request.Status);

        return NoContent();
    }
}
