using FluentValidation;

namespace Flowertrack.Application.Users.Commands.RemoveOrganizationUser;

public class RemoveOrganizationUserCommandValidator : AbstractValidator<RemoveOrganizationUserCommand>
{
    public RemoveOrganizationUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required");
    }
}
