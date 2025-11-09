using FluentValidation;

namespace Flowertrack.Application.Users.Commands.DeactivateServiceUser;

public class DeactivateServiceUserCommandValidator : AbstractValidator<DeactivateServiceUserCommand>
{
    public DeactivateServiceUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required for deactivation")
            .MaximumLength(500)
            .WithMessage("Reason must not exceed 500 characters");
    }
}
