namespace Flowertrack.Contracts.Machines.Requests;

public sealed record CompleteMaintenanceRequest
{
    public DateOnly CompletedDate { get; init; }
    public int? IntervalDays { get; init; }
}
