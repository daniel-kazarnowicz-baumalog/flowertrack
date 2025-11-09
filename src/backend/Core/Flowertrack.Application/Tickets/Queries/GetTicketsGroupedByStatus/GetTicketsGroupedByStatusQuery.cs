using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Queries.GetTicketsGroupedByStatus;

/// <summary>
/// Query to get tickets grouped by status with counts and sample tickets
/// </summary>
public sealed record GetTicketsGroupedByStatusQuery : IRequest<Result<List<TicketStatusGroupDto>>>
{
    /// <summary>
    /// ID of the user requesting the data (for authorization)
    /// </summary>
    public Guid RequestedBy { get; init; }

    /// <summary>
    /// Optional organization filter
    /// </summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>
    /// Number of sample tickets to include per status group (default: 5, max: 20)
    /// </summary>
    public int SampleSize { get; init; } = 5;
}
