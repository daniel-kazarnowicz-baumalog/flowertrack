using FluentValidation;

namespace Flowertrack.Application.Machines.Commands.RegisterMachine;

/// <summary>
/// Validator for RegisterMachineCommand
/// </summary>
public sealed class RegisterMachineCommandValidator : AbstractValidator<RegisterMachineCommand>
{
    public RegisterMachineCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Organization ID is required");

        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("Serial number is required")
            .MaximumLength(255).WithMessage("Serial number cannot exceed 255 characters");

        RuleFor(x => x.Brand)
            .MaximumLength(100).WithMessage("Brand cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Brand));

        RuleFor(x => x.Model)
            .MaximumLength(100).WithMessage("Model cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Model));

        RuleFor(x => x.Location)
            .MaximumLength(255).WithMessage("Location cannot exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Location));
    }
}
