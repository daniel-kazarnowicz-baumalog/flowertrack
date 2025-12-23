using System.Text.Json;
using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.ValueObjects;
using Flowertrack.Infrastructure.Mqtt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Infrastructure.Mqtt.Services;

/// <summary>
/// Processes machine log messages and persists them to the database
/// </summary>
public sealed class MqttLogProcessor : IMqttLogProcessor
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<MqttLogProcessor> _logger;

    public MqttLogProcessor(
        IApplicationDbContext dbContext,
        ILogger<MqttLogProcessor> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<bool> ProcessLogAsync(
        MqttMachineLogMessage message,
        string rawPayload,
        string topic,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate the message
            if (string.IsNullOrWhiteSpace(message.Token))
            {
                _logger.LogWarning("Received log message without token from topic {Topic}", topic);
                return false;
            }

            // Validate and parse the machine token
            if (!MachineApiKey.TryCreate(message.Token, out var apiKey))
            {
                _logger.LogWarning(
                    "Invalid machine token format: {Token} from topic {Topic}",
                    message.Token,
                    topic);
                return false;
            }

            // Find the machine by its API token
            var machine = await _dbContext.Machines
                .FirstOrDefaultAsync(
                    m => m.ApiToken != null && m.ApiToken.Value == apiKey.Value && !m.IsDeleted,
                    cancellationToken);

            if (machine == null)
            {
                _logger.LogWarning(
                    "Machine not found for token {Token} from topic {Topic}",
                    message.Token,
                    topic);
                return false;
            }

            // Determine log type and severity based on alarms
            var logType = DetermineLogType(message);
            var severity = DetermineSeverity(message);
            var status = DetermineStatus(message);

            // Extract alarm information
            string? alarmCode = null;
            string? alarmMessage = null;

            if (message.NewAlarms.Count > 0)
            {
                alarmCode = string.Join(",", message.NewAlarms);
                alarmMessage = $"New alarms detected: {alarmCode}";
            }
            else if (message.ActiveAlarms.Count > 0)
            {
                alarmCode = string.Join(",", message.ActiveAlarms);
            }

            // Create the machine log entity
            var machineLog = MachineLog.Create(
                machineId: machine.Id,
                logContent: rawPayload,
                logType: logType,
                status: status,
                alarmCode: alarmCode,
                alarmMessage: alarmMessage,
                severity: severity,
                machineTimestamp: new DateTimeOffset(message.Timestamp, TimeSpan.Zero));

            // Add to database context
            _dbContext.MachineLogs.Add(machineLog);

            // Save changes - this ensures data integrity before MQTT ACK
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully processed log for machine {MachineId} (Token: {Token}), " +
                "Cycles: {Cycles}, Active Alarms: {ActiveAlarmCount}, New Alarms: {NewAlarmCount}",
                machine.Id,
                message.Token,
                message.Cycles,
                message.ActiveAlarms.Count,
                message.NewAlarms.Count);

            // Update machine status if there are new alarms
            if (message.NewAlarms.Count > 0 && machine.Status != MachineStatus.Alarm)
            {
                try
                {
                    machine.ActivateAlarm($"New alarms: {alarmCode}", null);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    _logger.LogWarning(
                        "Activated alarm status for machine {MachineId} due to new alarms: {AlarmCode}",
                        machine.Id,
                        alarmCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to update machine status for machine {MachineId}",
                        machine.Id);
                    // Don't fail the entire operation if status update fails
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to process log message from topic {Topic}: {Error}",
                topic,
                ex.Message);
            return false;
        }
    }

    private static string DetermineLogType(MqttMachineLogMessage message)
    {
        if (message.NewAlarms.Count > 0)
        {
            return "ALARM";
        }

        if (message.ActiveAlarms.Count > 0)
        {
            return "WARNING";
        }

        if (message.Temperature.HasValue)
        {
            return "TELEMETRY";
        }

        return "INFO";
    }

    private static string DetermineSeverity(MqttMachineLogMessage message)
    {
        if (message.NewAlarms.Count > 0)
        {
            return "ERROR";
        }

        if (message.ActiveAlarms.Count > 0)
        {
            return "WARNING";
        }

        return "INFO";
    }

    private static string? DetermineStatus(MqttMachineLogMessage message)
    {
        if (message.NewAlarms.Count > 0 || message.ActiveAlarms.Count > 0)
        {
            return "ALARM";
        }

        return "NORMAL";
    }
}
