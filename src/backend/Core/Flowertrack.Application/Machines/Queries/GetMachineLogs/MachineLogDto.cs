namespace Flowertrack.Application.Machines.Queries.GetMachineLogs;

/// <summary>
/// DTO for machine log entries
/// </summary>
public sealed record MachineLogDto
{
    public Guid Id { get; init; }
    public Guid MachineId { get; init; }
    public DateTimeOffset ReceivedAt { get; init; }
    public DateTimeOffset? MachineTimestamp { get; init; }
    public string LogType { get; init; } = string.Empty;
    public string? Status { get; init; }
    public string? AlarmCode { get; init; }
    public string? AlarmMessage { get; init; }
    public string Severity { get; init; } = "INFO";
    public string LogContent { get; init; } = string.Empty;
}
