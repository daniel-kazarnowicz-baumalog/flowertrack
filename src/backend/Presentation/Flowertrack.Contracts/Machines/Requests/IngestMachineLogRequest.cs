using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Machines.Requests;

/// <summary>
/// Request to ingest a machine log entry
/// </summary>
public sealed record IngestMachineLogRequest
{
    /// <summary>
    /// Type of log entry (STATUS, ALARM, TELEMETRY, EVENT)
    /// </summary>
    [Required(ErrorMessage = "Log type is required")]
    [StringLength(50, ErrorMessage = "Log type cannot exceed 50 characters")]
    public string LogType { get; init; } = string.Empty;

    /// <summary>
    /// JSON content of the log entry
    /// </summary>
    [Required(ErrorMessage = "Log content is required")]
    public string LogContent { get; init; } = string.Empty;

    /// <summary>
    /// Optional machine status (Operational, Warning, Error, Maintenance, Inactive)
    /// </summary>
    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    public string? Status { get; init; }

    /// <summary>
    /// Optional alarm code for ALARM type logs
    /// </summary>
    [StringLength(100, ErrorMessage = "Alarm code cannot exceed 100 characters")]
    public string? AlarmCode { get; init; }

    /// <summary>
    /// Severity level (INFO, WARNING, ERROR, CRITICAL)
    /// </summary>
    [StringLength(50, ErrorMessage = "Severity cannot exceed 50 characters")]
    public string Severity { get; init; } = "INFO";
}
