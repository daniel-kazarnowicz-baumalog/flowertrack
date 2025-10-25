using Flowertrack.Domain.Common;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when a machine alarm is triggered.
/// This indicates a problem or warning condition detected by the machine.
/// </summary>
public sealed class MachineAlarmActivatedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the machine
    /// </summary>
    public Guid MachineId { get; }

    /// <summary>
    /// Reason or description of the alarm
    /// </summary>
    public string AlarmReason { get; }

    /// <summary>
    /// When the alarm was activated
    /// </summary>
    public DateTimeOffset ActivatedAt { get; }

    /// <summary>
    /// Severity level of the alarm (e.g., Critical, Warning, Info)
    /// </summary>
    public string Severity { get; }

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
