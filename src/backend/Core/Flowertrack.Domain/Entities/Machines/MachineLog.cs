using Flowertrack.Domain.Common;
using Flowertrack.Domain.Events;

namespace Flowertrack.Domain.Entities;

/// <summary>
/// Represents a log entry received from a machine via API.
/// Stores telemetry data, status updates, and alarms from production equipment.
/// </summary>
public sealed class MachineLog : AuditableEntity<Guid>
{
    /// <summary>
    /// The machine that sent this log
    /// </summary>
    public Guid MachineId { get; private set; }

    /// <summary>
    /// Timestamp when the log was received by the system
    /// </summary>
    public DateTimeOffset ReceivedAt { get; private set; }

    /// <summary>
    /// Timestamp from the machine when the log was generated (if provided)
    /// </summary>
    public DateTimeOffset? MachineTimestamp { get; private set; }

    /// <summary>
    /// Raw log content as JSON
    /// </summary>
    public string LogContent { get; private set; } = string.Empty;

    /// <summary>
    /// Log type: TELEMETRY, STATUS, ALARM, WARNING, INFO
    /// </summary>
    public string LogType { get; private set; } = string.Empty;

    /// <summary>
    /// Machine status at the time of log: NORMAL, ALARM, WARNING, MAINTENANCE
    /// </summary>
    public string? Status { get; private set; }

    /// <summary>
    /// Alarm code if this is an alarm log
    /// </summary>
    public string? AlarmCode { get; private set; }

    /// <summary>
    /// Human-readable alarm message
    /// </summary>
    public string? AlarmMessage { get; private set; }

    /// <summary>
    /// Severity level: INFO, WARNING, ERROR, CRITICAL
    /// </summary>
    public string Severity { get; private set; } = "INFO";

    /// <summary>
    /// Whether this log has been processed by the system
    /// </summary>
    public bool IsProcessed { get; private set; }

    /// <summary>
    /// Navigation property to machine
    /// </summary>
    public Machine Machine { get; private set; } = null!;

    // Private constructor for EF Core
    private MachineLog() : base(Guid.Empty)
    {
    }

    // Private constructor for domain logic
    private MachineLog(
        Guid machineId,
        string logContent,
        string logType,
        string? status,
        string? alarmCode,
        string? alarmMessage,
        string severity,
        DateTimeOffset? machineTimestamp) : base(Guid.NewGuid())
    {
        if (machineId == Guid.Empty)
        {
            throw new ArgumentException("Machine ID is required", nameof(machineId));
        }

        if (string.IsNullOrWhiteSpace(logContent))
        {
            throw new ArgumentException("Log content cannot be empty", nameof(logContent));
        }

        if (logContent.Length > 65535)
        {
            throw new ArgumentException("Log content cannot exceed 65535 characters", nameof(logContent));
        }

        if (string.IsNullOrWhiteSpace(logType))
        {
            throw new ArgumentException("Log type is required", nameof(logType));
        }

        ValidateLogType(logType);
        ValidateSeverity(severity);

        MachineId = machineId;
        ReceivedAt = DateTimeOffset.UtcNow;
        MachineTimestamp = machineTimestamp;
        LogContent = logContent;
        LogType = logType.ToUpperInvariant();
        Status = status?.ToUpperInvariant();
        AlarmCode = alarmCode;
        AlarmMessage = alarmMessage;
        Severity = severity.ToUpperInvariant();
        IsProcessed = false;

        // MachineLog is created by system, not a user
        SetCreatedAudit((Guid?)null);
    }

    /// <summary>
    /// Factory method to create a new machine log entry
    /// </summary>
    /// <param name="machineId">ID of the machine sending the log</param>
    /// <param name="logContent">Raw JSON content of the log</param>
    /// <param name="logType">Type of log (TELEMETRY, STATUS, ALARM, WARNING, INFO)</param>
    /// <param name="status">Current machine status</param>
    /// <param name="alarmCode">Alarm code if applicable</param>
    /// <param name="alarmMessage">Alarm message if applicable</param>
    /// <param name="severity">Severity level (INFO, WARNING, ERROR, CRITICAL)</param>
    /// <param name="machineTimestamp">Timestamp from the machine</param>
    /// <returns>New MachineLog instance</returns>
    public static MachineLog Create(
        Guid machineId,
        string logContent,
        string logType,
        string? status = null,
        string? alarmCode = null,
        string? alarmMessage = null,
        string severity = "INFO",
        DateTimeOffset? machineTimestamp = null)
    {
        var log = new MachineLog(
            machineId,
            logContent,
            logType,
            status,
            alarmCode,
            alarmMessage,
            severity,
            machineTimestamp);

        log.RaiseDomainEvent(new MachineLogReceivedEvent(
            log.Id,
            log.MachineId,
            log.LogType,
            log.Status,
            log.AlarmCode,
            log.Severity,
            log.ReceivedAt));

        return log;
    }

    /// <summary>
    /// Marks the log as processed
    /// </summary>
    public void MarkAsProcessed()
    {
        IsProcessed = true;
        SetUpdatedAudit((Guid?)null);
    }

    private static void ValidateLogType(string logType)
    {
        var validTypes = new[] { "TELEMETRY", "STATUS", "ALARM", "WARNING", "INFO" };
        if (!validTypes.Contains(logType.ToUpperInvariant()))
        {
            throw new ArgumentException(
                $"Invalid log type. Must be one of: {string.Join(", ", validTypes)}",
                nameof(logType));
        }
    }

    private static void ValidateSeverity(string severity)
    {
        var validSeverities = new[] { "INFO", "WARNING", "ERROR", "CRITICAL" };
        if (!validSeverities.Contains(severity.ToUpperInvariant()))
        {
            throw new ArgumentException(
                $"Invalid severity. Must be one of: {string.Join(", ", validSeverities)}",
                nameof(severity));
        }
    }
}
