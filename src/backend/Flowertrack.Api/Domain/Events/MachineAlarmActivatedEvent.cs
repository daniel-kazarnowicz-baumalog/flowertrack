namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a machine alarm is triggered.
/// This indicates a problem or warning condition detected by the machine.
/// </summary>
public sealed record MachineAlarmActivatedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the machine
    /// </summary>
    public Guid MachineId { get; init; }

    /// <summary>
    /// Reason or description of the alarm
    /// </summary>
    public string AlarmReason { get; init; }

    /// <summary>
    /// When the alarm was activated
    /// </summary>
    public DateTimeOffset ActivatedAt { get; init; }

    /// <summary>
    /// Severity level of the alarm (e.g., Critical, Warning, Info)
    /// </summary>
    public string Severity { get; init; }

    public MachineAlarmActivatedEvent(
        Guid machineId,
        string alarmReason,
        DateTimeOffset activatedAt,
        string severity)
        : base(machineId)
    {
        MachineId = machineId;
        AlarmReason = alarmReason;
        ActivatedAt = activatedAt;
        Severity = severity;
    }
}
