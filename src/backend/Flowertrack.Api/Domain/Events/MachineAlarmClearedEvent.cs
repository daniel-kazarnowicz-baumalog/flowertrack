namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a machine alarm is cleared or resolved.
/// This indicates that the alarm condition has been addressed.
/// </summary>
public sealed record MachineAlarmClearedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the machine
    /// </summary>
    public Guid MachineId { get; init; }

    /// <summary>
    /// Reason or explanation for clearing the alarm
    /// </summary>
    public string ClearedReason { get; init; }

    /// <summary>
    /// When the alarm was cleared
    /// </summary>
    public DateTimeOffset ClearedAt { get; init; }

    /// <summary>
    /// User who cleared the alarm
    /// </summary>
    public Guid ClearedBy { get; init; }

    public MachineAlarmClearedEvent(
        Guid machineId,
        string clearedReason,
        DateTimeOffset clearedAt,
        Guid clearedBy)
        : base(machineId)
    {
        MachineId = machineId;
        ClearedReason = clearedReason;
        ClearedAt = clearedAt;
        ClearedBy = clearedBy;
    }
}
