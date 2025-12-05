using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Queries.GetTicketComments;

/// <summary>
/// Query to retrieve comments for a specific ticket
/// </summary>
public sealed record GetTicketCommentsQuery : IRequest<Result<PagedResult<CommentDto>>>
{
    /// <summary>
    /// Ticket ID to get comments for
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// User ID requesting the comments (for authorization)
    /// </summary>
    public Guid RequestedBy { get; init; }

    /// <summary>
    /// Whether to include internal comments (service users only)
    /// </summary>
    public bool IncludeInternal { get; init; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Page size (default 50, max 200)
    /// </summary>
    public int PageSize { get; init; } = 50;
}
