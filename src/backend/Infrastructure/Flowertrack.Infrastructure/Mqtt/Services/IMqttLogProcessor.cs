using Flowertrack.Infrastructure.Mqtt.Models;

namespace Flowertrack.Infrastructure.Mqtt.Services;

/// <summary>
/// Interface for processing machine log messages received via MQTT
/// </summary>
public interface IMqttLogProcessor
{
    /// <summary>
    /// Processes a machine log message asynchronously
    /// </summary>
    /// <param name="message">The log message to process</param>
    /// <param name="rawPayload">The raw JSON payload</param>
    /// <param name="topic">The MQTT topic the message was received on</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if processing succeeded, false otherwise</returns>
    Task<bool> ProcessLogAsync(
        MqttMachineLogMessage message,
        string rawPayload,
        string topic,
        CancellationToken cancellationToken);
}
