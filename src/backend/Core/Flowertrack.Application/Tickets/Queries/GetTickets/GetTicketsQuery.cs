using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using MediatR;

namespace Flowertrack.Application.Tickets.Queries.GetTickets;

/// <summary>
/// Query to retrieve a filtered and paginated list of tickets
/// </summary>
public sealed record GetTicketsQuery : IRequest<Result<PagedResult<TicketListItemDto>>>
{
    /// <summary>
    /// User ID requesting the tickets (for authorization)
    /// </summary>
    public Guid RequestedBy { get; init; }

    /// <summary>
    /// Filter by organization ID (optional)
    /// </summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>
    /// Filter by machine ID (optional)
    /// </summary>
    public Guid? MachineId { get; init; }

    /// <summary>
    /// Filter by ticket status (optional)
    /// </summary>
    public TicketStatus? Status { get; init; }

    /// <summary>
    /// Filter by priority (optional)
    /// </summary>
    public Priority? Priority { get; init; }

    /// <summary>
    /// Filter by assigned user ID (optional)
    /// </summary>
    public Guid? AssignedToUserId { get; init; }

    /// <summary>
    /// Filter by created user ID (optional)
    /// </summary>
    public Guid? CreatedByUserId { get; init; }

    /// <summary>
    /// Filter by creation date from (optional)
    /// </summary>
    public DateTimeOffset? CreatedFrom { get; init; }

    /// <summary>
    /// Filter by creation date to (optional)
    /// </summary>
    public DateTimeOffset? CreatedTo { get; init; }

    /// <summary>
    /// Search text (searches in title and description, optional)
    /// </summary>
    public string? SearchText { get; init; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Page size (default 20, max 100)
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Sort by field (default: CreatedAt)
    /// </summary>
    public string SortBy { get; init; } = "CreatedAt";

    /// <summary>
    /// Sort direction (Asc or Desc, default: Desc)
    /// </summary>
    public string SortDirection { get; init; } = "Desc";
}
