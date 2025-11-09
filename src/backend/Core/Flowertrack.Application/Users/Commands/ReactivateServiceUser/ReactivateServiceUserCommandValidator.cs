using FluentValidation;

namespace Flowertrack.Application.Users.Commands.ReactivateServiceUser;

public class ReactivateServiceUserCommandValidator : AbstractValidator<ReactivateServiceUserCommand>
{
    public ReactivateServiceUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
