namespace Flowertrack.Infrastructure.Mqtt.Models;

/// <summary>
/// Represents a machine log message received via MQTT.
/// This matches the structure defined in the requirements.
/// </summary>
public sealed class MqttMachineLogMessage
{
    /// <summary>
    /// Machine token for authentication (12 characters, unique)
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the log was generated (UTC)
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Number of production cycles completed
    /// </summary>
    public long Cycles { get; set; }

    /// <summary>
    /// Currently active alarm codes
    /// </summary>
    public List<int> ActiveAlarms { get; set; } = new();

    /// <summary>
    /// New alarms since the last log
    /// </summary>
    public List<int> NewAlarms { get; set; } = new();

    /// <summary>
    /// Operator ID (if applicable)
    /// </summary>
    public int? OperatorId { get; set; }

    /// <summary>
    /// Current temperature reading (in Celsius)
    /// </summary>
    public decimal? Temperature { get; set; }

    /// <summary>
    /// Additional custom data specific to machine type
    /// </summary>
    public Dictionary<string, object>? CustomData { get; set; }
}
