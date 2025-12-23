using Flowertrack.Infrastructure.Mqtt.Models;

namespace Flowertrack.Infrastructure.Mqtt.Services;

/// <summary>
/// Interface for MQTT client service that handles machine log ingestion
/// </summary>
public interface IMqttLogIngestionService
{
    /// <summary>
    /// Starts the MQTT client and begins subscribing to machine log topics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task StartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Stops the MQTT client and disposes resources
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task StopAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the current connection status
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets statistics about message processing
    /// </summary>
    MqttIngestionStatistics GetStatistics();
}

/// <summary>
/// Statistics for MQTT log ingestion
/// </summary>
public sealed class MqttIngestionStatistics
{
    /// <summary>
    /// Total messages received
    /// </summary>
    public long TotalMessagesReceived { get; set; }

    /// <summary>
    /// Total messages successfully processed
    /// </summary>
    public long TotalMessagesProcessed { get; set; }

    /// <summary>
    /// Total messages failed
    /// </summary>
    public long TotalMessagesFailed { get; set; }

    /// <summary>
    /// Current queue size
    /// </summary>
    public int CurrentQueueSize { get; set; }

    /// <summary>
    /// Number of active workers
    /// </summary>
    public int ActiveWorkers { get; set; }

    /// <summary>
    /// Average processing time in milliseconds
    /// </summary>
    public double AverageProcessingTimeMs { get; set; }

    /// <summary>
    /// Messages per minute (last minute)
    /// </summary>
    public int MessagesPerMinute { get; set; }

    /// <summary>
    /// Whether the system is overloaded
    /// </summary>
    public bool IsOverloaded { get; set; }

    /// <summary>
    /// Timestamp of last message received
    /// </summary>
    public DateTimeOffset? LastMessageReceivedAt { get; set; }
}
