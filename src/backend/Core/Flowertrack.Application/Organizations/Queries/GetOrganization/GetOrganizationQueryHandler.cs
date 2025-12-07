using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Organizations.Queries.GetOrganization;

/// <summary>
/// Handler for GetOrganizationQuery
/// </summary>
public sealed class GetOrganizationQueryHandler : IRequestHandler<GetOrganizationQuery, Result<OrganizationDetailsDto>>
{
    private readonly IOrganizationRepository _organizationRepository;

    public GetOrganizationQueryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
    }

    public async Task<Result<OrganizationDetailsDto>> Handle(GetOrganizationQuery request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);

        if (organization == null || organization.IsDeleted)
        {
            return Result.Failure<OrganizationDetailsDto>($"Organization with ID {request.OrganizationId} was not found");
        }

        var dto = new OrganizationDetailsDto
        {
            Id = organization.Id,
            Name = organization.Name,
            Email = organization.Email,
            Phone = organization.Phone,
            Address = organization.Address,
            City = organization.City,
            PostalCode = organization.PostalCode,
            Country = organization.Country,
            ServiceStatus = organization.ServiceStatus.ToString(),
            ContractStartDate = organization.ContractStartDate,
            ContractEndDate = organization.ContractEndDate,
            Notes = organization.Notes,
            CreatedAt = organization.CreatedAt,
            UpdatedAt = organization.UpdatedAt
        };

        return Result.Success(dto);
    }
}
