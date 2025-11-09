using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Organizations.Commands.UpdateOrganization;

/// <summary>
/// Handler for UpdateOrganizationCommand
/// </summary>
public sealed class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, Result<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IUnitOfWork unitOfWork)
    {
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Guid>> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        // Get organization
        var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
        {
            return Result.Failure<Guid>($"Organization with ID {request.OrganizationId} was not found");
        }

        // Update contact information if provided
        if (request.Email != null || request.Phone != null || request.Address != null)
        {
            organization.UpdateContactInfo(request.Email, request.Phone, request.Address);
        }

        // TODO: Add Update method to Organization entity for Name, City, PostalCode, Country, Notes
        // For now, we'll update only contact info which is supported by the domain

        await _organizationRepository.UpdateAsync(organization, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(organization.Id);
    }
}
