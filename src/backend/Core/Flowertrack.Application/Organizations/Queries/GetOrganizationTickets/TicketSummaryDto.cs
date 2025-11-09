namespace Flowertrack.Application.Organizations.Queries.GetOrganizationTickets;

/// <summary>
/// DTO for ticket summary
/// </summary>
public sealed record TicketSummaryDto
{
    public Guid Id { get; init; }
    public string TicketNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public Guid? MachineId { get; init; }
    public Guid? AssignedToUserId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}
