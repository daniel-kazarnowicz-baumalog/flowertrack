using FluentValidation;

namespace Flowertrack.Application.Organizations.Commands.RegenerateApiKey;

/// <summary>
/// Validator for RegenerateApiKeyCommand
/// </summary>
public sealed class RegenerateApiKeyCommandValidator : AbstractValidator<RegenerateApiKeyCommand>
{
    public RegenerateApiKeyCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Organization ID is required");
    }
}
