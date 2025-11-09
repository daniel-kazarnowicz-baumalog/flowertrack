using FluentValidation;

namespace Flowertrack.Application.Machines.Commands.UpdateMachine;

/// <summary>
/// Validator for UpdateMachineCommand
/// </summary>
public sealed class UpdateMachineCommandValidator : AbstractValidator<UpdateMachineCommand>
{
    public UpdateMachineCommandValidator()
    {
        RuleFor(x => x.MachineId)
            .NotEmpty()
            .WithMessage("Machine ID is required");

        When(x => x.Brand != null, () =>
        {
            RuleFor(x => x.Brand)
                .MaximumLength(100)
                .WithMessage("Brand cannot exceed 100 characters");
        });

        When(x => x.Model != null, () =>
        {
            RuleFor(x => x.Model)
                .MaximumLength(100)
                .WithMessage("Model cannot exceed 100 characters");
        });

        When(x => x.Location != null, () =>
        {
            RuleFor(x => x.Location)
                .MaximumLength(255)
                .WithMessage("Location cannot exceed 255 characters");
        });
    }
}
