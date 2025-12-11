using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Flowertrack.Infrastructure.Configuration;
using Flowertrack.Infrastructure.Mqtt.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace Flowertrack.Infrastructure.Mqtt.Services;

/// <summary>
/// MQTT client service that subscribes to machine log topics and processes messages
/// using a worker pool architecture for high throughput
/// </summary>
public sealed class MqttLogIngestionService : IMqttLogIngestionService, IDisposable
{
    private readonly MqttOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MqttLogIngestionService> _logger;
    private readonly IMqttClient _mqttClient;
    private readonly Channel<QueuedMessage> _messageChannel;
    private readonly List<Task> _workerTasks = new();
    private readonly CancellationTokenSource _shutdownCts = new();
    private readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimits = new();
    private readonly Stopwatch _uptimeStopwatch = Stopwatch.StartNew();

    // Statistics tracking
    private long _totalMessagesReceived = 0;
    private long _totalMessagesProcessed = 0;
    private long _totalMessagesFailed = 0;
    private readonly ConcurrentQueue<long> _processingTimes = new();
    private readonly ConcurrentQueue<DateTimeOffset> _recentMessages = new();
    private DateTimeOffset? _lastMessageReceivedAt;

    public bool IsConnected => _mqttClient.IsConnected;

    public MqttLogIngestionService(
        IOptions<MqttOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<MqttLogIngestionService> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Create MQTT client
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        // Create bounded channel for message queue
        var channelOptions = new BoundedChannelOptions(_options.QueueCapacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest, // Drop oldest messages if queue is full
            SingleReader = false, // Multiple workers will read
            SingleWriter = false // Multiple MQTT threads might write
        };
        _messageChannel = Channel.CreateBounded<QueuedMessage>(channelOptions);

        _logger.LogInformation(
            "MQTT Log Ingestion Service created with {WorkerCount} workers and queue capacity {QueueCapacity}",
            _options.WorkerCount,
            _options.QueueCapacity);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("MQTT log ingestion is disabled in configuration");
            return;
        }

        _logger.LogInformation(
            "Starting MQTT Log Ingestion Service - connecting to {Server}:{Port}",
            _options.Server,
            _options.Port);

        try
        {
            // Configure MQTT client options
            var clientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_options.Server, _options.Port)
                .WithClientId(_options.ClientId ?? $"flowertrack-{Guid.NewGuid():N}")
                .WithCleanSession(false) // Maintain session for QoS
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(_options.KeepAliveSeconds))
                .WithTimeout(TimeSpan.FromSeconds(_options.ConnectionTimeoutSeconds));

            // Add authentication if provided
            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                clientOptions = clientOptions.WithCredentials(_options.Username, _options.Password);
            }

            // Add TLS if enabled
            if (_options.UseTls)
            {
                clientOptions = clientOptions.WithTlsOptions(o => o.UseTls());
            }

            // Set up event handlers
            _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
            _mqttClient.ConnectedAsync += OnConnectedAsync;
            _mqttClient.DisconnectedAsync += OnDisconnectedAsync;

            // Connect to MQTT broker
            var connectResult = await _mqttClient.ConnectAsync(clientOptions.Build(), cancellationToken);

            if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
            {
                throw new InvalidOperationException(
                    $"Failed to connect to MQTT broker: {connectResult.ResultCode} - {connectResult.ReasonString}");
            }

            _logger.LogInformation("Successfully connected to MQTT broker");

            // Subscribe to machine log topics with wildcard
            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f => f
                    .WithTopic(_options.TopicPattern)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce))
                .Build();

            var subscribeResult = await _mqttClient.SubscribeAsync(subscribeOptions, cancellationToken);

            foreach (var item in subscribeResult.Items)
            {
                if (item.ResultCode == MqttClientSubscribeResultCode.GrantedQoS0 ||
                    item.ResultCode == MqttClientSubscribeResultCode.GrantedQoS1 ||
                    item.ResultCode == MqttClientSubscribeResultCode.GrantedQoS2)
                {
                    _logger.LogInformation(
                        "Successfully subscribed to topic: {Topic} with QoS: {QoS}",
                        item.TopicFilter.Topic,
                        item.ResultCode);
                }
                else
                {
                    _logger.LogWarning(
                        "Failed to subscribe to topic: {Topic}, Result: {Result}",
                        item.TopicFilter.Topic,
                        item.ResultCode);
                }
            }

            // Start worker tasks
            for (int i = 0; i < _options.WorkerCount; i++)
            {
                var workerId = i + 1;
                var workerTask = Task.Run(
                    async () => await ProcessMessagesAsync(workerId, _shutdownCts.Token),
                    _shutdownCts.Token);
                _workerTasks.Add(workerTask);

                _logger.LogInformation("Started worker {WorkerId}", workerId);
            }

            _logger.LogInformation(
                "MQTT Log Ingestion Service started successfully with {WorkerCount} workers",
                _options.WorkerCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start MQTT Log Ingestion Service: {Error}", ex.Message);
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping MQTT Log Ingestion Service...");

        try
        {
            // Signal shutdown
            await _shutdownCts.CancelAsync();

            // Complete the channel to stop accepting new messages
            _messageChannel.Writer.Complete();

            // Wait for all workers to finish processing
            await Task.WhenAll(_workerTasks).ConfigureAwait(false);

            // Disconnect from MQTT broker
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
            }

            var stats = GetStatistics();
            _logger.LogInformation(
                "MQTT Log Ingestion Service stopped. Total messages received: {Received}, " +
                "processed: {Processed}, failed: {Failed}",
                stats.TotalMessagesReceived,
                stats.TotalMessagesProcessed,
                stats.TotalMessagesFailed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while stopping MQTT Log Ingestion Service: {Error}", ex.Message);
        }
    }

    public MqttIngestionStatistics GetStatistics()
    {
        // Clean up old messages (older than 1 minute)
        var oneMinuteAgo = DateTimeOffset.UtcNow.AddMinutes(-1);
        while (_recentMessages.TryPeek(out var timestamp) && timestamp < oneMinuteAgo)
        {
            _recentMessages.TryDequeue(out _);
        }

        // Calculate average processing time
        double avgProcessingTime = 0;
        if (_processingTimes.Count > 0)
        {
            avgProcessingTime = _processingTimes.Average();
        }

        // Calculate messages per minute
        int messagesPerMinute = _recentMessages.Count;

        // Check if overloaded
        var currentQueueSize = _messageChannel.Reader.Count;
        var overloadThreshold = (_options.QueueCapacity * _options.OverloadThresholdPercent) / 100;
        bool isOverloaded = currentQueueSize > overloadThreshold;

        return new MqttIngestionStatistics
        {
            TotalMessagesReceived = Interlocked.Read(ref _totalMessagesReceived),
            TotalMessagesProcessed = Interlocked.Read(ref _totalMessagesProcessed),
            TotalMessagesFailed = Interlocked.Read(ref _totalMessagesFailed),
            CurrentQueueSize = currentQueueSize,
            ActiveWorkers = _workerTasks.Count(t => !t.IsCompleted),
            AverageProcessingTimeMs = avgProcessingTime,
            MessagesPerMinute = messagesPerMinute,
            IsOverloaded = isOverloaded,
            LastMessageReceivedAt = _lastMessageReceivedAt
        };
    }

    private async Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        Interlocked.Increment(ref _totalMessagesReceived);
        _lastMessageReceivedAt = DateTimeOffset.UtcNow;
        _recentMessages.Enqueue(_lastMessageReceivedAt.Value);

        var topic = e.ApplicationMessage.Topic;
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

        try
        {
            // Extract machine token from topic (e.g., "machines/ABC123XYZ789/logs")
            var topicParts = topic.Split('/');
            string? machineToken = null;

            if (topicParts.Length >= 2)
            {
                machineToken = topicParts[1];
            }

            // Rate limiting check
            if (!string.IsNullOrWhiteSpace(machineToken) && !CheckRateLimit(machineToken))
            {
                _logger.LogWarning(
                    "Rate limit exceeded for machine {MachineToken}, dropping message",
                    machineToken);
                Interlocked.Increment(ref _totalMessagesFailed);
                return;
            }

            // Queue the message for processing
            var queuedMessage = new QueuedMessage
            {
                Topic = topic,
                Payload = payload,
                ReceivedAt = DateTimeOffset.UtcNow,
                MachineToken = machineToken
            };

            // Try to write to channel
            if (!await _messageChannel.Writer.WaitToWriteAsync(_shutdownCts.Token))
            {
                _logger.LogWarning("Message channel is closed, dropping message from {Topic}", topic);
                Interlocked.Increment(ref _totalMessagesFailed);
                return;
            }

            if (!_messageChannel.Writer.TryWrite(queuedMessage))
            {
                _logger.LogWarning(
                    "Failed to queue message from {Topic}, queue may be full (capacity: {Capacity})",
                    topic,
                    _options.QueueCapacity);
                Interlocked.Increment(ref _totalMessagesFailed);
                return;
            }

            // Log queue status if overloaded
            var stats = GetStatistics();
            if (stats.IsOverloaded)
            {
                _logger.LogWarning(
                    "System is overloaded - Queue: {QueueSize}/{Capacity} ({Percent}%), " +
                    "Messages/min: {MessagesPerMinute}",
                    stats.CurrentQueueSize,
                    _options.QueueCapacity,
                    (stats.CurrentQueueSize * 100.0) / _options.QueueCapacity,
                    stats.MessagesPerMinute);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MQTT message from topic {Topic}: {Error}", topic, ex.Message);
            Interlocked.Increment(ref _totalMessagesFailed);
        }
    }

    private async Task ProcessMessagesAsync(int workerId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker {WorkerId} started processing messages", workerId);

        try
        {
            await foreach (var message in _messageChannel.Reader.ReadAllAsync(cancellationToken))
            {
                var sw = Stopwatch.StartNew();

                try
                {
                    // Deserialize the message
                    var logMessage = JsonSerializer.Deserialize<MqttMachineLogMessage>(
                        message.Payload,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    if (logMessage == null)
                    {
                        _logger.LogWarning(
                            "Worker {WorkerId}: Failed to deserialize message from topic {Topic}",
                            workerId,
                            message.Topic);
                        Interlocked.Increment(ref _totalMessagesFailed);
                        continue;
                    }

                    // Process the log message with retries
                    bool success = await ProcessWithRetriesAsync(
                        logMessage,
                        message.Payload,
                        message.Topic,
                        cancellationToken);

                    if (success)
                    {
                        Interlocked.Increment(ref _totalMessagesProcessed);
                    }
                    else
                    {
                        Interlocked.Increment(ref _totalMessagesFailed);
                    }

                    sw.Stop();

                    // Track processing time
                    _processingTimes.Enqueue(sw.ElapsedMilliseconds);
                    if (_processingTimes.Count > 1000) // Keep only last 1000 samples
                    {
                        _processingTimes.TryDequeue(out _);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Worker {WorkerId}: Error processing message from topic {Topic}: {Error}",
                        workerId,
                        message.Topic,
                        ex.Message);
                    Interlocked.Increment(ref _totalMessagesFailed);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker {WorkerId} cancelled", workerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Worker {WorkerId} crashed: {Error}", workerId, ex.Message);
        }

        _logger.LogInformation("Worker {WorkerId} stopped", workerId);
    }

    private async Task<bool> ProcessWithRetriesAsync(
        MqttMachineLogMessage message,
        string rawPayload,
        string topic,
        CancellationToken cancellationToken)
    {
        for (int attempt = 1; attempt <= _options.MaxRetries + 1; attempt++)
        {
            try
            {
                // Create a new scope for each processing attempt to get a fresh DbContext
                using var scope = _scopeFactory.CreateScope();
                var logProcessor = scope.ServiceProvider.GetRequiredService<IMqttLogProcessor>();

                bool success = await logProcessor.ProcessLogAsync(
                    message,
                    rawPayload,
                    topic,
                    cancellationToken);

                if (success)
                {
                    return true;
                }

                if (attempt <= _options.MaxRetries)
                {
                    _logger.LogWarning(
                        "Processing failed for message from {Topic}, attempt {Attempt}/{MaxAttempts}",
                        topic,
                        attempt,
                        _options.MaxRetries + 1);

                    await Task.Delay(
                        TimeSpan.FromSeconds(_options.RetryDelaySeconds),
                        cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exception during processing attempt {Attempt}/{MaxAttempts} for {Topic}: {Error}",
                    attempt,
                    _options.MaxRetries + 1,
                    topic,
                    ex.Message);

                if (attempt <= _options.MaxRetries)
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(_options.RetryDelaySeconds),
                        cancellationToken);
                }
            }
        }

        _logger.LogError(
            "Failed to process message from {Topic} after {MaxRetries} retries",
            topic,
            _options.MaxRetries + 1);

        return false;
    }

    private bool CheckRateLimit(string machineToken)
    {
        var now = DateTimeOffset.UtcNow;
        var info = _rateLimits.GetOrAdd(machineToken, _ => new RateLimitInfo());

        lock (info)
        {
            // Reset counter if more than 1 minute has passed
            if ((now - info.WindowStart).TotalMinutes >= 1)
            {
                info.Count = 0;
                info.WindowStart = now;
            }

            info.Count++;

            return info.Count <= _options.MaxLogsPerMinutePerMachine;
        }
    }

    private Task OnConnectedAsync(MqttClientConnectedEventArgs e)
    {
        _logger.LogInformation(
            "Connected to MQTT broker - Result: {ResultCode}, Session: {IsSessionPresent}",
            e.ConnectResult.ResultCode,
            e.ConnectResult.IsSessionPresent);
        return Task.CompletedTask;
    }

    private Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs e)
    {
        _logger.LogWarning(
            "Disconnected from MQTT broker - Reason: {Reason}, Exception: {Exception}",
            e.Reason,
            e.Exception?.Message ?? "None");

        // Attempt to reconnect if not shutting down
        if (!_shutdownCts.Token.IsCancellationRequested && _options.Enabled)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                _logger.LogInformation("Attempting to reconnect to MQTT broker...");
                try
                {
                    // Reconnect using the same client
                    var options = new MqttClientOptionsBuilder()
                        .WithTcpServer(_options.Server, _options.Port)
                        .WithClientId(_options.ClientId ?? $"flowertrack-{Guid.NewGuid():N}")
                        .WithCleanSession(false)
                        .WithKeepAlivePeriod(TimeSpan.FromSeconds(_options.KeepAliveSeconds))
                        .WithTimeout(TimeSpan.FromSeconds(_options.ConnectionTimeoutSeconds));

                    if (!string.IsNullOrWhiteSpace(_options.Username))
                    {
                        options = options.WithCredentials(_options.Username, _options.Password);
                    }

                    if (_options.UseTls)
                    {
                        options = options.WithTlsOptions(o => o.UseTls());
                    }

                    await _mqttClient.ConnectAsync(options.Build(), _shutdownCts.Token);
                    _logger.LogInformation("Successfully reconnected to MQTT broker");

                    // Resubscribe to topics
                    var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                        .WithTopicFilter(f => f
                            .WithTopic(_options.TopicPattern)
                            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce))
                        .Build();

                    await _mqttClient.SubscribeAsync(subscribeOptions, _shutdownCts.Token);
                    _logger.LogInformation("Resubscribed to topic: {Topic}", _options.TopicPattern);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to reconnect to MQTT broker: {Error}", ex.Message);
                }
            });
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _mqttClient?.Dispose();
        _shutdownCts?.Dispose();
    }

    private sealed class QueuedMessage
    {
        public string Topic { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTimeOffset ReceivedAt { get; set; }
        public string? MachineToken { get; set; }
    }

    private sealed class RateLimitInfo
    {
        public int Count { get; set; }
        public DateTimeOffset WindowStart { get; set; } = DateTimeOffset.UtcNow;
    }
}
