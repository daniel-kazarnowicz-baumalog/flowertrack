using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Machines.Queries.GetMachineLogs;

/// <summary>
/// Handler for GetMachineLogsQuery.
/// Retrieves logs for a specific machine.
/// </summary>
public sealed class GetMachineLogsQueryHandler : IRequestHandler<GetMachineLogsQuery, Result<List<MachineLogDto>>>
{
    private readonly IMachineRepository _machineRepository;
    private readonly IMachineLogRepository _machineLogRepository;
    private readonly ILogger<GetMachineLogsQueryHandler> _logger;

    public GetMachineLogsQueryHandler(
        IMachineRepository machineRepository,
        IMachineLogRepository machineLogRepository,
        ILogger<GetMachineLogsQueryHandler> logger)
    {
        _machineRepository = machineRepository;
        _machineLogRepository = machineLogRepository;
        _logger = logger;
    }

    public async Task<Result<List<MachineLogDto>>> Handle(
        GetMachineLogsQuery request,
        CancellationToken cancellationToken)
    {
        // Verify machine exists
        var machineExists = await _machineRepository.ExistsAsync(request.MachineId, cancellationToken);
        if (!machineExists)
        {
            return Result.Failure<List<MachineLogDto>>($"Machine with ID {request.MachineId} was not found");
        }

        _logger.LogDebug("Retrieving logs for machine {MachineId}", request.MachineId);

        // Get logs from repository
        var logs = await _machineLogRepository.GetByMachineIdAsync(request.MachineId, cancellationToken);

        var dtos = logs.Select(log => new MachineLogDto
        {
            Id = log.Id,
            MachineId = log.MachineId,
            ReceivedAt = log.ReceivedAt,
            MachineTimestamp = log.MachineTimestamp,
            LogType = log.LogType,
            Status = log.Status,
            AlarmCode = log.AlarmCode,
            AlarmMessage = log.AlarmMessage,
            Severity = log.Severity,
            LogContent = log.LogContent
        }).ToList();

        return Result.Success(dtos);
    }
}
