using Flowertrack.Application.Tickets.Commands.AddTicketComment;
using Flowertrack.Application.Tickets.Commands.UpdateTicketComment;
using Flowertrack.Application.Tickets.Commands.DeleteTicketComment;
using Flowertrack.Application.Tickets.Queries.GetTicketComments;
using Flowertrack.Contracts.Common;
using Flowertrack.Contracts.Tickets.Requests;
using Flowertrack.Contracts.Tickets.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flowertrack.Api.Controllers;

/// <summary>
/// Controller for managing ticket comments
/// </summary>
[ApiController]
[Route("api/tickets/{ticketId:guid}/comments")]
[Authorize(Policy = "RequireAuthenticatedUser")]
public class TicketCommentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TicketCommentsController> _logger;

    public TicketCommentsController(
        IMediator mediator,
        ILogger<TicketCommentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get comments for a ticket
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="includeInternal">Include internal comments (service users only)</param>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 50, max 200)</param>
    /// <returns>Paginated list of comments</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetCommentsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetComments(
        Guid ticketId,
        [FromQuery] bool includeInternal = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        // Only service users can include internal comments
        var userTypeClaim = User.FindFirst("user_type")?.Value;
        var isServiceUser = userTypeClaim == "ServiceUser";
        
        var query = new GetTicketCommentsQuery
        {
            TicketId = ticketId,
            RequestedBy = userId,
            IncludeInternal = includeInternal && isServiceUser, // Force false for non-service users
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to get comments for ticket {TicketId}: {Error}", ticketId, result.Error);
            return result.Error == "Ticket not found" 
                ? NotFound(new ErrorResponse(result.Error)) 
                : BadRequest(new ErrorResponse(result.Error ?? "Failed to get comments"));
        }

        var response = new GetCommentsResponse
        {
            Comments = result.Value!.Items.Select(c => new CommentResponse
            {
                Id = c.Id,
                TicketId = c.TicketId,
                UserId = c.UserId,
                UserName = c.UserName,
                Content = c.Content,
                IsInternal = c.IsInternal,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList(),
            Pagination = new PaginationMetadata
            {
                PageNumber = result.Value.PageNumber,
                PageSize = result.Value.PageSize,
                TotalPages = result.Value.TotalPages,
                TotalCount = result.Value.TotalCount,
                HasPreviousPage = result.Value.PageNumber > 1,
                HasNextPage = result.Value.PageNumber < result.Value.TotalPages
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Add a comment to a ticket
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="request">Comment content</param>
    /// <returns>Created comment ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddComment(
        Guid ticketId,
        [FromBody] AddCommentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        // Only service users can add internal comments
        var userTypeClaim = User.FindFirst("user_type")?.Value;
        var isServiceUser = userTypeClaim == "ServiceUser";

        if (request.IsInternal && !isServiceUser)
        {
            return Forbid();
        }

        var command = new AddTicketCommentCommand
        {
            TicketId = ticketId,
            Content = request.Content,
            UserId = userId,
            IsInternal = request.IsInternal
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to add comment to ticket {TicketId}: {Error}", ticketId, result.Error);
            return result.Error == "Ticket not found" 
                ? NotFound(new ErrorResponse(result.Error)) 
                : BadRequest(new ErrorResponse(result.Error ?? "Failed to add comment"));
        }

        return CreatedAtAction(
            nameof(GetComments),
            new { ticketId },
            result.Value);
    }

    /// <summary>
    /// Update a comment on a ticket
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="commentId">Comment ID</param>
    /// <param name="request">Updated comment content</param>
    /// <returns>No content on success</returns>
    [HttpPatch("{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateComment(
        Guid ticketId,
        Guid commentId,
        [FromBody] UpdateCommentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        var command = new UpdateTicketCommentCommand
        {
            CommentId = commentId,
            Content = request.Content,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to update comment {CommentId}: {Error}", commentId, result.Error);
            
            if (result.Error == "Comment not found")
                return NotFound(new ErrorResponse(result.Error));
            
            if (result.Error == "Only the comment author can edit the comment")
                return StatusCode(403, new ErrorResponse(result.Error));
            
            return BadRequest(new ErrorResponse(result.Error ?? "Failed to update comment"));
        }

        return NoContent();
    }

    /// <summary>
    /// Delete a comment from a ticket
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="commentId">Comment ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(
        Guid ticketId,
        Guid commentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        var command = new DeleteTicketCommentCommand
        {
            CommentId = commentId,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to delete comment {CommentId}: {Error}", commentId, result.Error);
            return result.Error == "Comment not found" 
                ? NotFound(new ErrorResponse(result.Error)) 
                : BadRequest(new ErrorResponse(result.Error ?? "Failed to delete comment"));
        }

        return NoContent();
    }
}
