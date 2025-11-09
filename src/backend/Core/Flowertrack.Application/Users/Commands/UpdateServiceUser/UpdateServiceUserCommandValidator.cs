using FluentValidation;

namespace Flowertrack.Application.Users.Commands.UpdateServiceUser;

public class UpdateServiceUserCommandValidator : AbstractValidator<UpdateServiceUserCommand>
{
    public UpdateServiceUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(100)
            .WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100)
            .WithMessage("Last name must not exceed 100 characters");

        When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(50)
                .WithMessage("Phone number must not exceed 50 characters");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Specialization), () =>
        {
            RuleFor(x => x.Specialization)
                .MaximumLength(200)
                .WithMessage("Specialization must not exceed 200 characters");
        });
    }
}
