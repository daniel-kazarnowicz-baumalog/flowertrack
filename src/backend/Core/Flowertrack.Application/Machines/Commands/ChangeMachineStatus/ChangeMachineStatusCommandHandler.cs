using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.ChangeMachineStatus;

/// <summary>
/// Handler for ChangeMachineStatusCommand
/// </summary>
public sealed class ChangeMachineStatusCommandHandler : IRequestHandler<ChangeMachineStatusCommand, Result>
{
    private readonly IMachineRepository _machineRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeMachineStatusCommandHandler(
        IMachineRepository machineRepository,
        IUnitOfWork unitOfWork)
    {
        _machineRepository = machineRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeMachineStatusCommand request, CancellationToken cancellationToken)
    {
        // Get machine
        var machine = await _machineRepository.GetByIdAsync(request.MachineId, cancellationToken);
        if (machine == null)
        {
            return Result.Failure($"Machine with ID {request.MachineId} was not found");
        }

        try
        {
            // Update status
            machine.UpdateStatus(request.NewStatus, request.Reason, updatedBy: null);

            // Save
            await _machineRepository.UpdateAsync(machine, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
