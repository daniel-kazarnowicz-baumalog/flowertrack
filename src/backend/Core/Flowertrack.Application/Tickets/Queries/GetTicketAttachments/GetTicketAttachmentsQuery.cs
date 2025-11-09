using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Queries.GetTicketAttachments;

/// <summary>
/// Query to retrieve attachments for a specific ticket
/// </summary>
public sealed record GetTicketAttachmentsQuery : IRequest<Result<PagedResult<AttachmentDto>>>
{
    /// <summary>
    /// Ticket ID to get attachments for
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// User ID requesting the attachments (for authorization)
    /// </summary>
    public Guid RequestedBy { get; init; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Page size (default 50, max 100)
    /// </summary>
    public int PageSize { get; init; } = 50;
}
