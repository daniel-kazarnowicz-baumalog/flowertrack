using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Organizations.Queries.GetOrganizationUsers;

/// <summary>
/// Handler for GetOrganizationUsersQuery
/// </summary>
public sealed class GetOrganizationUsersQueryHandler 
    : IRequestHandler<GetOrganizationUsersQuery, Result<List<OrganizationUserSummaryDto>>>
{
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public GetOrganizationUsersQueryHandler(
        IOrganizationUserRepository organizationUserRepository,
        IOrganizationRepository organizationRepository)
    {
        _organizationUserRepository = organizationUserRepository ?? throw new ArgumentNullException(nameof(organizationUserRepository));
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
    }

    public async Task<Result<List<OrganizationUserSummaryDto>>> Handle(
        GetOrganizationUsersQuery request, 
        CancellationToken cancellationToken)
    {
        // Verify organization exists
        var organizationExists = await _organizationRepository.ExistsAsync(request.OrganizationId, cancellationToken);
        if (!organizationExists)
        {
            return Result.Failure<List<OrganizationUserSummaryDto>>(
                $"Organization with ID {request.OrganizationId} was not found");
        }

        // Get users
        var users = await _organizationUserRepository.GetByOrganizationIdAsync(request.OrganizationId, cancellationToken);

        var userDtos = users.Select(u => new OrganizationUserSummaryDto
        {
            Id = u.Id,
            Email = u.Email.Value,
            FullName = $"{u.FirstName} {u.LastName}",
            Role = u.Role,
            Status = u.Status.ToString(),
            IsActivated = u.IsActivated,
            CreatedAt = u.CreatedAt
        }).ToList();

        return Result.Success(userDtos);
    }
}
