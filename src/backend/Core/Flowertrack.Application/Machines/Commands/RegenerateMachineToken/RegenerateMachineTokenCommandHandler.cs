using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Machines.Commands.RegenerateMachineToken;

/// <summary>
/// Handler for RegenerateMachineTokenCommand
/// </summary>
public sealed class RegenerateMachineTokenCommandHandler : IRequestHandler<RegenerateMachineTokenCommand, Result<string>>
{
    private readonly IMachineRepository _machineRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegenerateMachineTokenCommandHandler(
        IMachineRepository machineRepository,
        IUnitOfWork unitOfWork)
    {
        _machineRepository = machineRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(RegenerateMachineTokenCommand request, CancellationToken cancellationToken)
    {
        // Get machine
        var machine = await _machineRepository.GetByIdAsync(request.MachineId, cancellationToken);
        if (machine == null)
        {
            return Result.Failure<string>($"Machine with ID {request.MachineId} was not found");
        }

        // Regenerate token
        machine.RegenerateApiToken(request.Reason, updatedBy: null);

        // Save
        await _machineRepository.UpdateAsync(machine, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(machine.ApiToken!.Value);
    }
}
