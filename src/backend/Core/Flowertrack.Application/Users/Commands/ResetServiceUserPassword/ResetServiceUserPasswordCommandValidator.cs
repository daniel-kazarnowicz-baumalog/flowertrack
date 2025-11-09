using FluentValidation;

namespace Flowertrack.Application.Users.Commands.ResetServiceUserPassword;

public class ResetServiceUserPasswordCommandValidator : AbstractValidator<ResetServiceUserPasswordCommand>
{
    public ResetServiceUserPasswordCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
