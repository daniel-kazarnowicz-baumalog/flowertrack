using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using Flowertrack.Domain.ValueObjects;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.ScheduleMaintenance;

public sealed class ScheduleMaintenanceCommandHandler : IRequestHandler<ScheduleMaintenanceCommand, Result>
{
    private readonly IMachineRepository _machineRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ScheduleMaintenanceCommandHandler(IMachineRepository machineRepository, IUnitOfWork unitOfWork)
    {
        _machineRepository = machineRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ScheduleMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var machine = await _machineRepository.GetByIdAsync(request.MachineId, cancellationToken);
        if (machine == null)
            return Result.Failure($"Machine with ID {request.MachineId} was not found");

        MaintenanceInterval? interval = request.IntervalDays.HasValue 
            ? MaintenanceInterval.Create(1, request.IntervalDays.Value, $"Custom {request.IntervalDays.Value}-day interval") 
            : null;

        machine.ScheduleMaintenance(request.ScheduledDate, interval, updatedBy: null);

        await _machineRepository.UpdateAsync(machine, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
