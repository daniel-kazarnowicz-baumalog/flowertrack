namespace Flowertrack.Application.Users.Queries.GetServiceUserStats;

/// <summary>
/// DTO containing service user statistics.
/// US-031: Displayed on technician list and details page.
/// </summary>
public record ServiceUserStatsDto
{
    /// <summary>
    /// The service user's ID
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Total number of tickets ever assigned to this user
    /// </summary>
    public int TotalTicketsAssigned { get; init; }

    /// <summary>
    /// Number of tickets currently in progress
    /// </summary>
    public int TicketsInProgress { get; init; }

    /// <summary>
    /// Number of tickets resolved in the last 30 days
    /// </summary>
    public int TicketsResolvedLast30Days { get; init; }

    /// <summary>
    /// Number of tickets closed in the last 30 days
    /// </summary>
    public int TicketsClosedLast30Days { get; init; }

    /// <summary>
    /// Average time in hours to resolve a ticket (from assignment to resolved)
    /// </summary>
    public double? AverageResolutionTimeHours { get; init; }

    /// <summary>
    /// When the user last performed an action (resolved ticket, added comment, etc.)
    /// </summary>
    public DateTimeOffset? LastActivityAt { get; init; }

    /// <summary>
    /// Number of tickets by priority breakdown
    /// </summary>
    public TicketsByPriorityDto TicketsByPriority { get; init; } = new();
}

/// <summary>
/// Breakdown of tickets by priority
/// </summary>
public record TicketsByPriorityDto
{
    public int Critical { get; init; }
    public int High { get; init; }
    public int Medium { get; init; }
    public int Low { get; init; }
}
