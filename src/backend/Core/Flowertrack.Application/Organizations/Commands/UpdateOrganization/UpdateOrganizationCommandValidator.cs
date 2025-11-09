using FluentValidation;

namespace Flowertrack.Application.Organizations.Commands.UpdateOrganization;

/// <summary>
/// Validator for UpdateOrganizationCommand
/// </summary>
public sealed class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    public UpdateOrganizationCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required");

        RuleFor(x => x.Name)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.Name))
            .WithMessage("Organization name cannot exceed 255 characters");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Invalid email format")
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.Phone)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Phone cannot exceed 50 characters");

        RuleFor(x => x.Address)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("Address cannot exceed 255 characters");

        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.City))
            .WithMessage("City cannot exceed 100 characters");

        RuleFor(x => x.PostalCode)
            .MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.PostalCode))
            .WithMessage("Postal code cannot exceed 20 characters");

        RuleFor(x => x.Country)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Country))
            .WithMessage("Country cannot exceed 100 characters");
    }
}
