using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.UpdateMachine;

/// <summary>
/// Handler for UpdateMachineCommand
/// </summary>
public sealed class UpdateMachineCommandHandler : IRequestHandler<UpdateMachineCommand, Result>
{
    private readonly IMachineRepository _machineRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMachineCommandHandler(
        IMachineRepository machineRepository,
        IUnitOfWork unitOfWork)
    {
        _machineRepository = machineRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateMachineCommand request, CancellationToken cancellationToken)
    {
        // Get machine
        var machine = await _machineRepository.GetByIdAsync(request.MachineId, cancellationToken);
        if (machine == null)
        {
            return Result.Failure($"Machine with ID {request.MachineId} was not found");
        }

        // Update machine
        machine.Update(request.Brand, request.Model, request.Location, updatedBy: null);

        // Save
        await _machineRepository.UpdateAsync(machine, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
