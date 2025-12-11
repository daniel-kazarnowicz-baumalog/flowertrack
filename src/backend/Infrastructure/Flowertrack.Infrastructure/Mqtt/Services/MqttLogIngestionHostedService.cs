using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Infrastructure.Mqtt.Services;

/// <summary>
/// Background service that manages the lifecycle of MQTT log ingestion
/// </summary>
public sealed class MqttLogIngestionHostedService : IHostedService
{
    private readonly IMqttLogIngestionService _mqttService;
    private readonly ILogger<MqttLogIngestionHostedService> _logger;

    public MqttLogIngestionHostedService(
        IMqttLogIngestionService mqttService,
        ILogger<MqttLogIngestionHostedService> logger)
    {
        _mqttService = mqttService ?? throw new ArgumentNullException(nameof(mqttService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MQTT Log Ingestion Hosted Service is starting");

        try
        {
            await _mqttService.StartAsync(cancellationToken);
            _logger.LogInformation("MQTT Log Ingestion Hosted Service started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start MQTT Log Ingestion Hosted Service: {Error}", ex.Message);
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MQTT Log Ingestion Hosted Service is stopping");

        try
        {
            await _mqttService.StopAsync(cancellationToken);
            _logger.LogInformation("MQTT Log Ingestion Hosted Service stopped successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping MQTT Log Ingestion Hosted Service: {Error}", ex.Message);
        }
    }
}
