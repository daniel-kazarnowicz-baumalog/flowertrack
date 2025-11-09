using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Organizations.Commands.RegenerateApiKey;

/// <summary>
/// Handler for RegenerateApiKeyCommand
/// </summary>
public sealed class RegenerateApiKeyCommandHandler : IRequestHandler<RegenerateApiKeyCommand, Result<string>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegenerateApiKeyCommandHandler(
        IOrganizationRepository organizationRepository,
        IUnitOfWork unitOfWork)
    {
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<string>> Handle(RegenerateApiKeyCommand request, CancellationToken cancellationToken)
    {
        // Get organization
        var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
        {
            return Result.Failure<string>($"Organization with ID {request.OrganizationId} was not found");
        }

        // Generate new API key
        var newApiKey = organization.GenerateApiKey();

        await _organizationRepository.UpdateAsync(organization, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(newApiKey);
    }
}
