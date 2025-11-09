using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Machines.Queries.GetMachineLogs;

public sealed class GetMachineLogsQueryHandler : IRequestHandler<GetMachineLogsQuery, Result<List<MachineLogDto>>>
{
    private readonly IMachineRepository _machineRepository;

    public GetMachineLogsQueryHandler(IMachineRepository machineRepository)
    {
        _machineRepository = machineRepository;
    }

    public async Task<Result<List<MachineLogDto>>> Handle(GetMachineLogsQuery request, CancellationToken cancellationToken)
    {
        // Verify machine exists
        var machineExists = await _machineRepository.ExistsAsync(request.MachineId, cancellationToken);
        if (!machineExists)
        {
            return Result.Failure<List<MachineLogDto>>($"Machine with ID {request.MachineId} was not found");
        }

        // TODO: Implement actual log retrieval from event store or audit log table
        // For now, return empty list as placeholder
        var logs = new List<MachineLogDto>();

        return Result.Success(logs);
    }
}
