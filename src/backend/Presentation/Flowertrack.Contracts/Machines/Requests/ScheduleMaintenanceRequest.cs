namespace Flowertrack.Contracts.Machines.Requests;

public sealed record ScheduleMaintenanceRequest
{
    public DateOnly ScheduledDate { get; init; }
    public int? IntervalDays { get; init; }
}
