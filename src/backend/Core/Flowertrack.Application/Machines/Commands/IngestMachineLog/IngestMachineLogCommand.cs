using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.IngestMachineLog;

/// <summary>
/// Command to ingest a log entry from a machine.
/// This is called when a machine sends telemetry/status/alarm data via API token.
/// US-028: Generowanie tokenu API dla maszyn
/// US-029: Przeglądanie statusów maszyn i alertów
/// </summary>
public sealed record IngestMachineLogCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// The machine ID (resolved from API token by middleware)
    /// </summary>
    public Guid MachineId { get; init; }

    /// <summary>
    /// Raw log content as JSON string
    /// </summary>
    public string LogContent { get; init; } = string.Empty;

    /// <summary>
    /// Type of log: TELEMETRY, STATUS, ALARM, WARNING, INFO
    /// </summary>
    public string LogType { get; init; } = "INFO";

    /// <summary>
    /// Current machine status: NORMAL, ALARM, WARNING, MAINTENANCE
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Alarm code if this is an alarm log
    /// </summary>
    public string? AlarmCode { get; init; }

    /// <summary>
    /// Human-readable alarm message
    /// </summary>
    public string? AlarmMessage { get; init; }

    /// <summary>
    /// Severity level: INFO, WARNING, ERROR, CRITICAL
    /// </summary>
    public string Severity { get; init; } = "INFO";

    /// <summary>
    /// Timestamp from the machine when the log was generated
    /// </summary>
    public DateTimeOffset? MachineTimestamp { get; init; }
}
