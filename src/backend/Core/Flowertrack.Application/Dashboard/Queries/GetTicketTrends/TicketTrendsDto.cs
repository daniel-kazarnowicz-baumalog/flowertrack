namespace Flowertrack.Application.Dashboard.Queries.GetTicketTrends;

/// <summary>
/// DTO containing ticket trends data
/// US-007: Wykres trendów zgłoszeń
/// </summary>
public sealed record TicketTrendsDto
{
    /// <summary>
    /// List of dates in the trend period (ISO format)
    /// </summary>
    public IReadOnlyList<string> Dates { get; init; } = [];

    /// <summary>
    /// Number of tickets opened on each date
    /// </summary>
    public IReadOnlyList<int> Opened { get; init; } = [];

    /// <summary>
    /// Number of tickets resolved on each date
    /// </summary>
    public IReadOnlyList<int> Resolved { get; init; } = [];

    /// <summary>
    /// Number of tickets closed on each date
    /// </summary>
    public IReadOnlyList<int> Closed { get; init; } = [];

    /// <summary>
    /// Breakdown by priority for the entire period
    /// </summary>
    public PriorityBreakdownDto ByPriority { get; init; } = new();
}

/// <summary>
/// DTO containing priority breakdown for tickets
/// </summary>
public sealed record PriorityBreakdownDto
{
    /// <summary>
    /// Number of critical priority tickets
    /// </summary>
    public int Critical { get; init; }

    /// <summary>
    /// Number of high priority tickets
    /// </summary>
    public int High { get; init; }

    /// <summary>
    /// Number of medium priority tickets
    /// </summary>
    public int Medium { get; init; }

    /// <summary>
    /// Number of low priority tickets
    /// </summary>
    public int Low { get; init; }
}
