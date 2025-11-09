using FluentValidation;

namespace Flowertrack.Application.Machines.Commands.ChangeMachineStatus;

/// <summary>
/// Validator for ChangeMachineStatusCommand
/// </summary>
public sealed class ChangeMachineStatusCommandValidator : AbstractValidator<ChangeMachineStatusCommand>
{
    public ChangeMachineStatusCommandValidator()
    {
        RuleFor(x => x.MachineId)
            .NotEmpty()
            .WithMessage("Machine ID is required");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid machine status");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required for status change")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}
