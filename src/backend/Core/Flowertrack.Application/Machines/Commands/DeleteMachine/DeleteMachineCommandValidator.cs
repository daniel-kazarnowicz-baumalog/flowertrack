using FluentValidation;

namespace Flowertrack.Application.Machines.Commands.DeleteMachine;

/// <summary>
/// Validator for DeleteMachineCommand
/// </summary>
public sealed class DeleteMachineCommandValidator : AbstractValidator<DeleteMachineCommand>
{
    public DeleteMachineCommandValidator()
    {
        RuleFor(x => x.MachineId)
            .NotEmpty()
            .WithMessage("Machine ID is required");
    }
}
