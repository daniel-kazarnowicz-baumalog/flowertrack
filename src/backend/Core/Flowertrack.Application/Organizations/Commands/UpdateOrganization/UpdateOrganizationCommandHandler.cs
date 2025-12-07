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

        // Check for name uniqueness if name is being changed
        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != organization.Name)
        {
            var nameExists = await _organizationRepository.NameExistsAsync(request.Name, request.OrganizationId, cancellationToken);
            if (nameExists)
            {
                return Result.Failure<Guid>($"Organization with name '{request.Name}' already exists");
            }
        }

        // Update organization with all provided fields
        organization.Update(
            request.Name,
            request.Email,
            request.Phone,
            request.Address,
            request.City,
            request.PostalCode,
            request.Country,
            request.Notes);

        await _organizationRepository.UpdateAsync(organization, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(organization.Id);
    }
}
