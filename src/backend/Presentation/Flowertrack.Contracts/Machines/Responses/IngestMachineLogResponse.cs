namespace Flowertrack.Contracts.Machines.Responses;

/// <summary>
/// Response returned after successfully ingesting a machine log
/// </summary>
public sealed record IngestMachineLogResponse
{
    /// <summary>
    /// The ID of the created machine log entry
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The machine ID that the log belongs to
    /// </summary>
    public Guid MachineId { get; init; }

    /// <summary>
    /// Timestamp when the log was received
    /// </summary>
    public DateTimeOffset ReceivedAt { get; init; }

    /// <summary>
    /// Type of log entry
    /// </summary>
    public string LogType { get; init; } = string.Empty;

    /// <summary>
    /// Indicates if the log triggered a machine status update
    /// </summary>
    public bool StatusUpdated { get; init; }
}
