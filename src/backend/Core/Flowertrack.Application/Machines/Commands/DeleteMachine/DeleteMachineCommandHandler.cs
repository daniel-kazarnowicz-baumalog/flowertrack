using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.DeleteMachine;

/// <summary>
/// Handler for DeleteMachineCommand
/// </summary>
public sealed class DeleteMachineCommandHandler : IRequestHandler<DeleteMachineCommand, Result>
{
    private readonly IMachineRepository _machineRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMachineCommandHandler(
        IMachineRepository machineRepository,
        IUnitOfWork unitOfWork)
    {
        _machineRepository = machineRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteMachineCommand request, CancellationToken cancellationToken)
    {
        // Get machine
        var machine = await _machineRepository.GetByIdAsync(request.MachineId, cancellationToken);
        if (machine == null)
        {
            return Result.Failure($"Machine with ID {request.MachineId} was not found");
        }

        // Soft delete
        machine.Delete(deletedBy: null);

        // Save
        await _machineRepository.UpdateAsync(machine, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
