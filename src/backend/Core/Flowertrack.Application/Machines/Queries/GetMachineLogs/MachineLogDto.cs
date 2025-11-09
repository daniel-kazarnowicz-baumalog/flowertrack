namespace Flowertrack.Application.Machines.Queries.GetMachineLogs;

public sealed record MachineLogDto
{
    public Guid Id { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTimeOffset Timestamp { get; init; }
    public Guid? PerformedBy { get; init; }
}
