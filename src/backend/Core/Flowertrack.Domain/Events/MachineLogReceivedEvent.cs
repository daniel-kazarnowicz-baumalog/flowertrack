using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Event raised when a new log is received from a machine.
/// This event can trigger status updates, alarm notifications, or telemetry processing.
/// </summary>
public sealed class MachineLogReceivedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the log entry
    /// </summary>
    public Guid LogId { get; }

    /// <summary>
    /// ID of the machine that sent the log
    /// </summary>
    public Guid MachineId { get; }

    /// <summary>
    /// Type of log (TELEMETRY, STATUS, ALARM, WARNING, INFO)
    /// </summary>
    public string LogType { get; }

    /// <summary>
    /// Machine status at the time of log
    /// </summary>
    public string? Status { get; }

    /// <summary>
    /// Alarm code if this is an alarm log
    /// </summary>
    public string? AlarmCode { get; }

    /// <summary>
    /// Severity level of the log
    /// </summary>
    public string Severity { get; }

    /// <summary>
    /// When the log was received by the system
    /// </summary>
    public DateTimeOffset ReceivedAt { get; }

    public MachineLogReceivedEvent(
        Guid logId,
        Guid machineId,
        string logType,
        string? status,
        string? alarmCode,
        string severity,
        DateTimeOffset receivedAt)
        : base(logId)
    {
        LogId = logId;
        MachineId = machineId;
        LogType = logType;
        Status = status;
        AlarmCode = alarmCode;
        Severity = severity;
        ReceivedAt = receivedAt;
    }
}
