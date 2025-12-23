using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Infrastructure.Configuration;

/// <summary>
/// Configuration options for MQTT broker connection
/// </summary>
public sealed class MqttOptions
{
    /// <summary>
    /// MQTT broker server address (e.g., "mqtt.example.com" or "localhost")
    /// </summary>
    [Required(ErrorMessage = "MQTT Server is required")]
    public string Server { get; set; } = string.Empty;

    /// <summary>
    /// MQTT broker port (default: 1883 for TCP, 8883 for TLS)
    /// </summary>
    [Range(1, 65535, ErrorMessage = "MQTT Port must be between 1 and 65535")]
    public int Port { get; set; } = 1883;

    /// <summary>
    /// Username for MQTT authentication (optional)
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password for MQTT authentication (optional)
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Client ID for MQTT connection (auto-generated if not provided)
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Whether to use TLS/SSL for MQTT connection
    /// </summary>
    public bool UseTls { get; set; } = false;

    /// <summary>
    /// Topic pattern to subscribe to (e.g., "machines/+/logs")
    /// </summary>
    [Required(ErrorMessage = "Topic pattern is required")]
    public string TopicPattern { get; set; } = "machines/+/logs";

    /// <summary>
    /// Number of worker threads for processing incoming messages
    /// </summary>
    [Range(1, 100, ErrorMessage = "Worker count must be between 1 and 100")]
    public int WorkerCount { get; set; } = 10;

    /// <summary>
    /// Maximum queue size for buffering incoming messages
    /// </summary>
    [Range(100, 100000, ErrorMessage = "Queue capacity must be between 100 and 100,000")]
    public int QueueCapacity { get; set; } = 10000;

    /// <summary>
    /// Maximum number of retries for failed message processing
    /// </summary>
    [Range(0, 10, ErrorMessage = "Max retries must be between 0 and 10")]
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Delay between retry attempts in seconds
    /// </summary>
    [Range(1, 60, ErrorMessage = "Retry delay must be between 1 and 60 seconds")]
    public int RetryDelaySeconds { get; set; } = 5;

    /// <summary>
    /// Connection timeout in seconds
    /// </summary>
    [Range(5, 120, ErrorMessage = "Connection timeout must be between 5 and 120 seconds")]
    public int ConnectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Keep alive interval in seconds
    /// </summary>
    [Range(10, 300, ErrorMessage = "Keep alive must be between 10 and 300 seconds")]
    public int KeepAliveSeconds { get; set; } = 60;

    /// <summary>
    /// Whether MQTT log ingestion is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Maximum logs per minute per machine (for rate limiting)
    /// </summary>
    [Range(1, 10000, ErrorMessage = "Max logs per minute must be between 1 and 10,000")]
    public int MaxLogsPerMinutePerMachine { get; set; } = 200;

    /// <summary>
    /// Threshold for overload detection (percentage of queue capacity)
    /// </summary>
    [Range(50, 100, ErrorMessage = "Overload threshold must be between 50 and 100")]
    public int OverloadThresholdPercent { get; set; } = 80;
}
