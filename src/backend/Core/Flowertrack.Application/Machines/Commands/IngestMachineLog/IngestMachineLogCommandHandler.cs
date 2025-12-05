using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using Flowertrack.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Machines.Commands.IngestMachineLog;

/// <summary>
/// Handler for IngestMachineLogCommand.
/// Creates a log entry and optionally updates machine status based on the log content.
/// </summary>
public sealed class IngestMachineLogCommandHandler
    : IRequestHandler<IngestMachineLogCommand, Result<Guid>>
{
    private readonly IMachineLogRepository _machineLogRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IngestMachineLogCommandHandler> _logger;

    public IngestMachineLogCommandHandler(
        IMachineLogRepository machineLogRepository,
        IMachineRepository machineRepository,
        IUnitOfWork unitOfWork,
        ILogger<IngestMachineLogCommandHandler> logger)
    {
        _machineLogRepository = machineLogRepository;
        _machineRepository = machineRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        IngestMachineLogCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug(
                "Ingesting machine log for machine {MachineId}, type {LogType}, severity {Severity}",
                request.MachineId,
                request.LogType,
                request.Severity);

            // Create the machine log entry
            var machineLog = MachineLog.Create(
                machineId: request.MachineId,
                logContent: request.LogContent,
                logType: request.LogType,
                status: request.Status,
                alarmCode: request.AlarmCode,
                alarmMessage: request.AlarmMessage,
                severity: request.Severity,
                machineTimestamp: request.MachineTimestamp);

            await _machineLogRepository.AddAsync(machineLog, cancellationToken);

            // If this is an alarm, update the machine status
            if (request.LogType.Equals("ALARM", StringComparison.OrdinalIgnoreCase) ||
                request.Status?.Equals("ALARM", StringComparison.OrdinalIgnoreCase) == true)
            {
                await UpdateMachineStatusToAlarmAsync(
                    request.MachineId,
                    request.AlarmCode,
                    request.AlarmMessage,
                    cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully ingested machine log {LogId} for machine {MachineId}",
                machineLog.Id,
                request.MachineId);

            return Result.Success(machineLog.Id);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument for machine log ingestion: {Message}", ex.Message);
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting machine log for machine {MachineId}", request.MachineId);
            return Result.Failure<Guid>($"Failed to ingest machine log: {ex.Message}");
        }
    }

    private async Task UpdateMachineStatusToAlarmAsync(
        Guid machineId,
        string? alarmCode,
        string? alarmMessage,
        CancellationToken cancellationToken)
    {
        try
        {
            var machine = await _machineRepository.GetByIdAsync(machineId, cancellationToken);
            if (machine == null)
            {
                _logger.LogWarning("Machine {MachineId} not found for status update", machineId);
                return;
            }

            // Only update if not already in Alarm status
            if (machine.Status != MachineStatus.Alarm)
            {
                var reason = string.IsNullOrWhiteSpace(alarmMessage)
                    ? $"Alarm received: {alarmCode ?? "Unknown"}"
                    : $"Alarm: {alarmMessage} (Code: {alarmCode ?? "N/A"})";

                machine.UpdateStatus(MachineStatus.Alarm, reason);
                await _machineRepository.UpdateAsync(machine, cancellationToken);

                _logger.LogWarning(
                    "Machine {MachineId} status updated to ALARM. Code: {AlarmCode}, Message: {AlarmMessage}",
                    machineId,
                    alarmCode,
                    alarmMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update machine status for machine {MachineId}", machineId);
            // Don't fail the whole operation if status update fails
        }
    }
}
