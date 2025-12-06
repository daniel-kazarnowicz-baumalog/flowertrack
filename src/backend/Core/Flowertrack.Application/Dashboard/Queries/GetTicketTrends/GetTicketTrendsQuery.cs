using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Dashboard.Queries.GetTicketTrends;

/// <summary>
/// Query to retrieve ticket trends for the last N days
/// US-007: Wykres trendów zgłoszeń
/// </summary>
public sealed record GetTicketTrendsQuery : IRequest<Result<TicketTrendsDto>>
{
    /// <summary>
    /// Number of days to retrieve trends for (default 30)
    /// </summary>
    public int Days { get; init; } = 30;

    /// <summary>
    /// Optional organization ID to filter trends (null for all organizations)
    /// </summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>
    /// User ID making the request (for authorization)
    /// </summary>
    public Guid RequestedBy { get; init; }
}
