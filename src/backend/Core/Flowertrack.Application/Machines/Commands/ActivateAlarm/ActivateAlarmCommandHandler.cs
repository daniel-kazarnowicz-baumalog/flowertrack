using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.ActivateAlarm;

public sealed class ActivateAlarmCommandHandler : IRequestHandler<ActivateAlarmCommand, Result>
{
    private readonly IMachineRepository _machineRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateAlarmCommandHandler(IMachineRepository machineRepository, IUnitOfWork unitOfWork)
    {
        _machineRepository = machineRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ActivateAlarmCommand request, CancellationToken cancellationToken)
    {
        var machine = await _machineRepository.GetByIdAsync(request.MachineId, cancellationToken);
        if (machine == null)
            return Result.Failure($"Machine with ID {request.MachineId} was not found");

        machine.ActivateAlarm(request.Reason, updatedBy: null);

        await _machineRepository.UpdateAsync(machine, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
