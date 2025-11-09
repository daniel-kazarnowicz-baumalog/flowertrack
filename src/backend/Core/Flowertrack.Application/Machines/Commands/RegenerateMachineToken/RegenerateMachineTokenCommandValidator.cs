using FluentValidation;

namespace Flowertrack.Application.Machines.Commands.RegenerateMachineToken;

/// <summary>
/// Validator for RegenerateMachineTokenCommand
/// </summary>
public sealed class RegenerateMachineTokenCommandValidator : AbstractValidator<RegenerateMachineTokenCommand>
{
    public RegenerateMachineTokenCommandValidator()
    {
        RuleFor(x => x.MachineId)
            .NotEmpty()
            .WithMessage("Machine ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required for token regeneration")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}
