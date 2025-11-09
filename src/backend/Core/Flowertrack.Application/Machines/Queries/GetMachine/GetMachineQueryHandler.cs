using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Machines.Queries.GetMachine;

public sealed class GetMachineQueryHandler : IRequestHandler<GetMachineQuery, Result<MachineDetailsDto>>
{
    private readonly IMachineRepository _machineRepository;

    public GetMachineQueryHandler(IMachineRepository machineRepository)
    {
        _machineRepository = machineRepository;
    }

    public async Task<Result<MachineDetailsDto>> Handle(GetMachineQuery request, CancellationToken cancellationToken)
    {
        var machine = await _machineRepository.GetByIdAsync(request.MachineId, cancellationToken);
        if (machine == null)
        {
            return Result.Failure<MachineDetailsDto>($"Machine with ID {request.MachineId} was not found");
        }

        var dto = new MachineDetailsDto
        {
            Id = machine.Id,
            OrganizationId = machine.OrganizationId,
            SerialNumber = machine.SerialNumber,
            Brand = machine.Brand,
            Model = machine.Model,
            Location = machine.Location,
            Status = machine.Status.ToString(),
            ApiToken = machine.ApiToken?.Value,
            LastMaintenanceDate = machine.LastMaintenanceDate,
            NextMaintenanceDate = machine.NextMaintenanceDate,
            MaintenanceIntervalId = machine.MaintenanceIntervalId,
            CreatedAt = machine.CreatedAt,
            CreatedBy = machine.CreatedBy,
            UpdatedAt = machine.UpdatedAt,
            UpdatedBy = machine.UpdatedBy
        };

        return Result.Success(dto);
    }
}
