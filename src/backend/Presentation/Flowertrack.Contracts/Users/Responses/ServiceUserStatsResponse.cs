namespace Flowertrack.Contracts.Users.Responses;

/// <summary>
/// Response DTO for service user statistics.
/// US-031: Statystyki serwisanta
/// </summary>
public record ServiceUserStatsResponse(
    Guid UserId,
    int TotalTicketsAssigned,
    int TicketsInProgress,
    int TicketsResolvedLast30Days,
    int TicketsClosedLast30Days,
    double? AverageResolutionTimeHours,
    DateTimeOffset? LastActivityAt,
    TicketsByPriorityResponse TicketsByPriority);

/// <summary>
/// Breakdown of tickets by priority
/// </summary>
public record TicketsByPriorityResponse(
    int Critical,
    int High,
    int Medium,
    int Low);
