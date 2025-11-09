using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Auth.Queries.GetCurrentOrganizationUser;

/// <summary>
/// Handler for getting current authenticated organization user
/// </summary>
public sealed class GetCurrentOrganizationUserQueryHandler : IRequestHandler<GetCurrentOrganizationUserQuery, Result<OrganizationUserDto>>
{
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public GetCurrentOrganizationUserQueryHandler(
        IOrganizationUserRepository organizationUserRepository,
        IOrganizationRepository organizationRepository)
    {
        _organizationUserRepository = organizationUserRepository ?? throw new ArgumentNullException(nameof(organizationUserRepository));
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
    }

    public async Task<Result<OrganizationUserDto>> Handle(GetCurrentOrganizationUserQuery request, CancellationToken cancellationToken)
    {
        // Get organization user
        var organizationUser = await _organizationUserRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (organizationUser == null)
        {
            return Result.Failure<OrganizationUserDto>($"Organization user with ID {request.UserId} was not found");
        }

        // Get organization name
        var organization = await _organizationRepository.GetByIdAsync(organizationUser.OrganizationId);
        string organizationName = organization?.Name ?? "Unknown Organization";

        // Map to DTO
        var dto = new OrganizationUserDto
        {
            Id = organizationUser.Id,
            OrganizationId = organizationUser.OrganizationId,
            OrganizationName = organizationName,
            Email = organizationUser.Email.Value,
            FullName = $"{organizationUser.FirstName} {organizationUser.LastName}",
            Role = organizationUser.Role, // Use the Role property from entity
            Status = organizationUser.Status.ToString(),
            IsActivated = organizationUser.IsActivated
        };

        return Result.Success(dto);
    }
}
