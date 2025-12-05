using Flowertrack.Application.Comments.Commands.AddComment;
using Flowertrack.Application.Comments.Commands.UpdateComment;
using Flowertrack.Application.Comments.Commands.DeleteComment;
using Flowertrack.Application.Comments.Queries.GetComments;
using Flowertrack.Application.Tickets.Commands.AddNote;
using Flowertrack.Application.Tickets.Commands.AssignTicket;
using Flowertrack.Application.Tickets.Commands.CreateTicket;
using Flowertrack.Application.Tickets.Commands.DeleteTicket;
using Flowertrack.Application.Tickets.Commands.UpdateTicket;
using Flowertrack.Application.Tickets.Commands.UpdateTicketStatus;
using Flowertrack.Application.Tickets.Queries.GetTicket;
using Flowertrack.Application.Tickets.Queries.GetTicketHistory;
using Flowertrack.Application.Tickets.Queries.GetTickets;
using Flowertrack.Application.Tickets.Queries.GetTicketsGroupedByStatus;
using Flowertrack.Contracts.Common;
using Flowertrack.Contracts.Comments; // For AddCommentRequest
using Flowertrack.Contracts.Tickets.Requests;
using Flowertrack.Contracts.Tickets.Responses;
using Flowertrack.Contracts.Tickets;
using Flowertrack.Api.Contracts.Requests;
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
[Authorize(Policy = "RequireAuthenticatedUser")] // Default policy: any authenticated user (service or organization)
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
    /// Get a filtered and paginated list of tickets
    /// US-015: Pobieranie listy zgłoszeń serwisowych z filtrami
    /// </summary>
    /// <param name="organizationId">Filter by organization ID</param>
    /// <param name="machineId">Filter by machine ID</param>
    /// <param name="status">Filter by status (0=New, 1=Accepted, 2=InProgress, 3=Resolved, 4=Closed, 5=Reopened)</param>
    /// <param name="priority">Filter by priority (0=Low, 1=Medium, 2=High, 3=Critical)</param>
    /// <param name="assignedToUserId">Filter by assigned user ID</param>
    /// <param name="createdByUserId">Filter by creator user ID</param>
    /// <param name="createdFrom">Filter by creation date from</param>
    /// <param name="createdTo">Filter by creation date to</param>
    /// <param name="searchText">Search in title and description</param>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 20, max 100)</param>
    /// <param name="sortBy">Sort by field (default CreatedAt)</param>
    /// <param name="sortDirection">Sort direction: Asc or Desc (default Desc)</param>
    /// <returns>Paginated list of tickets</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetTicketsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTickets(
        [FromQuery] Guid? organizationId = null,
        [FromQuery] Guid? machineId = null,
        [FromQuery] int? status = null,
        [FromQuery] int? priority = null,
        [FromQuery] Guid? assignedToUserId = null,
        [FromQuery] Guid? createdByUserId = null,
        [FromQuery] DateTimeOffset? createdFrom = null,
        [FromQuery] DateTimeOffset? createdTo = null,
        [FromQuery] string? searchText = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] string sortDirection = "Desc")
    {
        // Get current user ID from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("User ID not found in claims or invalid format");
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        var query = new GetTicketsQuery
        {
            RequestedBy = userId,
            OrganizationId = organizationId,
            MachineId = machineId,
            Status = status.HasValue ? (TicketStatus)status.Value : null,
            Priority = priority.HasValue ? (Priority)priority.Value : null,
            AssignedToUserId = assignedToUserId,
            CreatedByUserId = createdByUserId,
            CreatedFrom = createdFrom,
            CreatedTo = createdTo,
            SearchText = searchText,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to retrieve tickets: {Error}", result.Error);
            return BadRequest(new ErrorResponse(result.Error!));
        }

        var pagedResult = result.Value!;

        // Map from Application DTO (with enums) to Contracts DTO (with ints)
        var response = new GetTicketsResponse
        {
            Items = pagedResult.Items.Select(item => new Flowertrack.Contracts.Tickets.TicketListItemDto
            {
                Id = item.Id,
                TicketNumber = item.TicketNumber,
                Title = item.Title,
                Status = (int)item.Status,
                Priority = (int)item.Priority,
                OrganizationId = item.OrganizationId,
                OrganizationName = item.OrganizationName,
                MachineId = item.MachineId,
                MachineSerialNumber = item.MachineSerialNumber,
                AssignedToUserId = item.AssignedToUserId,
                AssignedToUserName = item.AssignedToUserName,
                CreatedByUserId = item.CreatedByUserId,
                CreatedByUserName = item.CreatedByUserName,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                ResolvedAt = item.ResolvedAt,
                ClosedAt = item.ClosedAt
            }).ToList(),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalCount = pagedResult.TotalCount,
            TotalPages = pagedResult.TotalPages,
            HasPreviousPage = pagedResult.HasPreviousPage,
            HasNextPage = pagedResult.HasNextPage
        };

        _logger.LogInformation(
            "Retrieved {Count} tickets (page {Page}/{TotalPages}) for user {UserId}",
            response.Items.Count,
            response.PageNumber,
            response.TotalPages,
            userId);

        return Ok(response);
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
    /// 
    /// **Authorization:** Only service users can change ticket status.
    /// </remarks>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "RequireServiceUser")] // Only service users can change status
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

    /// <summary>
    /// Assign a ticket to a service technician
    /// US-015: Przypisanie zgłoszenia serwisowego do technika
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="request">Assignment details</param>
    /// <returns>No content on success</returns>
    /// <remarks>
    /// **Authorization:** Only service users can assign tickets.
    /// </remarks>
    [HttpPatch("{id:guid}/assign")]
    [Authorize(Policy = "RequireServiceUser")] // Only service users can assign tickets
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignTicket(
        [FromRoute] Guid id,
        [FromBody] AssignTicketRequest request)
    {
        // Get current user ID from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("User ID not found in claims or invalid format");
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        _logger.LogInformation(
            "Assigning ticket {TicketId} to user {AssignedToUserId}",
            id,
            request.AssignedToUserId);

        var command = new AssignTicketCommand(id, request.AssignedToUserId, userId);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Failed to assign ticket {TicketId}: {Error}",
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

            if (result.Error.Contains("closed", StringComparison.OrdinalIgnoreCase) ||
                result.Error.Contains("cannot assign", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new ErrorResponse(result.Error));
            }

            return BadRequest(new ErrorResponse(result.Error));
        }

        _logger.LogInformation(
            "Successfully assigned ticket {TicketId} to user {AssignedToUserId}",
            id,
            request.AssignedToUserId);

        return NoContent();
    }

    /// <summary>
    /// Get tickets grouped by status with counts and sample tickets
    /// </summary>
    /// <param name="organizationId">Optional organization filter</param>
    /// <param name="sampleSize">Number of sample tickets per group (default: 5, max: 20)</param>
    /// <returns>Tickets grouped by status</returns>
    [HttpGet("grouped-by-status")]
    [ProducesResponseType(typeof(GetTicketsGroupedByStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTicketsGroupedByStatus(
        [FromQuery] Guid? organizationId = null,
        [FromQuery] int sampleSize = 5)
    {
        // Get current user ID from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("User ID not found in claims or invalid format");
            return Unauthorized(new ErrorResponse("User authentication failed"));
        }

        var query = new GetTicketsGroupedByStatusQuery
        {
            RequestedBy = userId,
            OrganizationId = organizationId,
            SampleSize = sampleSize
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to get tickets grouped by status: {Error}", result.Error);
            return BadRequest(new ErrorResponse(result.Error!));
        }

        var groups = result.Value!;

        // Map from Application DTO (with enums) to Contracts DTO (with ints and names)
        var response = new GetTicketsGroupedByStatusResponse
        {
            Groups = groups.Select(group => new TicketStatusGroup
            {
                Status = (int)group.Status,
                StatusName = group.Status.ToString(),
                Count = group.Count,
                SampleTickets = group.SampleTickets.Select(sample => new TicketSample
                {
                    Id = sample.Id,
                    TicketNumber = sample.TicketNumber,
                    Title = sample.Title,
                    Priority = (int)sample.Priority,
                    PriorityName = sample.Priority.ToString(),
                    OrganizationName = sample.OrganizationName,
                    MachineSerialNumber = sample.MachineSerialNumber,
                    CreatedAt = sample.CreatedAt
                }).ToList()
            }).ToList()
        };

        _logger.LogInformation(
            "Retrieved {GroupCount} status groups with total {TotalTickets} tickets for user {UserId}",
            response.Groups.Count,
            response.Groups.Sum(g => g.Count),
            userId);

        return Ok(response);
    }

    /// <summary>
    /// Get ticket history/timeline
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="includeInternal">Include internal notes (service users only)</param>
    /// <param name="pageNumber">Page number for pagination (optional)</param>
    /// <param name="pageSize">Page size for pagination (optional)</param>
    /// <returns>Ticket history entries</returns>
    [HttpGet("{id:guid}/history")]
    [ProducesResponseType(typeof(TicketHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHistory(
        Guid id,
        [FromQuery] bool includeInternal = false,
        [FromQuery] int? pageNumber = null,
        [FromQuery] int? pageSize = null)
    {
        _logger.LogInformation("Getting history for ticket {TicketId}", id);

        var query = new GetTicketHistoryQuery(
            id,
            includeInternal,
            pageNumber,
            pageSize);

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to get ticket history: {Error}", result.Error);
            return NotFound(new ErrorResponse(result.Error ?? "Ticket not found"));
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Add a comment to a ticket
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="request">Comment content</param>
    /// <returns>Created comment</returns>
    [HttpPost("{id:guid}/comments")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddComment(
        Guid id,
        [FromBody] AddCommentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        _logger.LogInformation("Adding comment to ticket {TicketId} by user {UserId}", id, userId);

        var command = new AddCommentCommand
        {
            TicketId = id,
            Content = request.Content,
            IsInternal = request.IsInternal
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to add comment: {Error}", result.Error);
            return BadRequest(new ErrorResponse(result.Error ?? "Failed to add comment"));
        }

        return CreatedAtAction(
            nameof(GetHistory), // Temporarily point to History until GetComments is exposed
            new { id },
            result.Value);
    }

    /// <summary>
    /// Add an internal note to a ticket (service users only)
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="request">Note content</param>
    /// <returns>Created note</returns>
    [HttpPost("{id:guid}/notes")]
    [Authorize(Roles = "ServiceTechnician,ServiceAdministrator")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddNote(
        Guid id,
        [FromBody] AddNoteRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userNameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
        var userTypeClaim = User.FindFirst("user_type")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        _logger.LogInformation("Adding internal note to ticket {TicketId} by user {UserId}", id, userId);

        var command = new AddCommentCommand
        {
            TicketId = id,
            Content = request.Content,
            IsInternal = true
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to add note: {Error}", result.Error);
            return BadRequest(new ErrorResponse(result.Error ?? "Failed to add note"));
        }

        return CreatedAtAction(
            nameof(GetHistory),
            new { id },
            result.Value);
    }
            userNameClaim ?? "Unknown User",
            userTypeClaim ?? "ServiceUser");

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to add note: {Error}", result.Error);
            
            if (result.Error?.Contains("Only service users") == true)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(result.Error));
            }
            
            return BadRequest(new ErrorResponse(result.Error ?? "Failed to add note"));
        }

        return CreatedAtAction(
            nameof(GetHistory),
            new { id },
            result.Value);
    }

    /// <summary>
    /// Get comments for a ticket
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paginated list of comments</returns>
    [HttpGet("{id:guid}/comments")]
    [ProducesResponseType(typeof(GetCommentsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetComments(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetCommentsForTicketQuery
        {
            TicketId = id,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new ErrorResponse(result.Error ?? "Ticket not found"));
        }

        var comments = result.Value;
        
        // Map to Contract Response
        var response = new GetCommentsResponse
        {
            Items = comments.Items.Select(c => new CommentResponse(
                c.Id,
                c.TicketId,
                c.UserId,
                c.AuthorName,
                c.Content,
                c.IsInternal,
                c.CreatedAt,
                c.UpdatedAt,
                c.CanEdit,
                c.CanDelete
            )).ToList(),
            Pagination = new PaginationMetadata
            {
                PageNumber = comments.PageNumber,
                PageSize = comments.PageSize,
                TotalCount = comments.TotalCount,
                TotalPages = comments.TotalPages,
                HasNextPage = comments.HasNextPage,
                HasPreviousPage = comments.HasPreviousPage
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Update a comment
    /// </summary>
    /// <param name="id">Ticket ID (ignored in route for command, but useful for consistency)</param>
    /// <param name="commentId">Comment ID</param>
    /// <param name="request">Update details</param>
    [HttpPut("{id:guid}/comments/{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateComment(
        Guid id,
        Guid commentId,
        [FromBody] UpdateCommentRequest request)
    {
        var command = new UpdateCommentCommand
        {
            CommentId = commentId,
            Content = request.Content
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true) return NotFound(new ErrorResponse(result.Error));
            return BadRequest(new ErrorResponse(result.Error ?? "Failed to update comment"));
        }

        return NoContent();
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    [HttpDelete("{id:guid}/comments/{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(Guid id, Guid commentId)
    {
        var command = new DeleteCommentCommand(commentId);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true) return NotFound(new ErrorResponse(result.Error));
            return BadRequest(new ErrorResponse(result.Error ?? "Failed to delete comment"));
        }

        return NoContent();
    }

    /// <summary>
    /// Upload an attachment to a ticket
    /// </summary>
    [HttpPost("{id:guid}/attachments")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadAttachment(
        Guid id,
        [FromForm] UploadAttachmentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        var command = new Flowertrack.Application.Attachments.Commands.UploadAttachment.UploadTicketAttachmentCommand
        {
            TicketId = id,
            UploadedBy = userId,
            FileName = request.File.FileName,
            ContentType = request.File.ContentType,
            FileSize = request.File.Length,
            FileStream = request.File.OpenReadStream()
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true) return NotFound(new ErrorResponse(result.Error));
            return BadRequest(new ErrorResponse(result.Error ?? "Failed to upload attachment"));
        }

        return CreatedAtAction(
            nameof(GetHistory), // Attachments are listed in history
            new { id },
            result.Value);
    }

    /// <summary>
    /// Delete an attachment
    /// </summary>
    [HttpDelete("{id:guid}/attachments/{attachmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAttachment(Guid id, Guid attachmentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        var command = new Flowertrack.Application.Attachments.Commands.DeleteAttachment.DeleteTicketAttachmentCommand
        {
            AttachmentId = attachmentId,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
             if (result.Error?.Contains("not found") == true) return NotFound(new ErrorResponse(result.Error));
             return BadRequest(new ErrorResponse(result.Error ?? "Failed to delete attachment"));
        }

        return NoContent();
    }
}
