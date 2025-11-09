using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Users.Commands.RemoveOrganizationUser;

public class RemoveOrganizationUserCommandHandler : IRequestHandler<RemoveOrganizationUserCommand, Result<Unit>>
{
    private readonly IOrganizationUserRepository _organizationUserRepository;

    public RemoveOrganizationUserCommandHandler(IOrganizationUserRepository organizationUserRepository)
    {
        _organizationUserRepository = organizationUserRepository;
    }

    public async Task<Result<Unit>> Handle(RemoveOrganizationUserCommand request, CancellationToken ct)
    {
        var user = await _organizationUserRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result.Failure<Unit>("Organization user not found");

        if (user.OrganizationId != request.OrganizationId)
            return Result.Failure<Unit>("User does not belong to the specified organization");

        // Check if this is the last admin in the organization
        if (user.Role == OrganizationUserRole.Admin || user.Role == OrganizationUserRole.Owner)
        {
            var orgUsers = await _organizationUserRepository.GetByOrganizationIdAsync(request.OrganizationId, ct);
            var adminCount = orgUsers.Count(u => 
                (u.Role == OrganizationUserRole.Admin || u.Role == OrganizationUserRole.Owner) && 
                !u.IsDeleted);
            
            if (adminCount <= 1)
                return Result.Failure<Unit>("Cannot remove the last administrator from organization");
        }

        // Soft delete - use public Delete method if available, otherwise set fields directly
        user.Delete(); // Assuming AuditableEntity has public Delete() method

        await _organizationUserRepository.UpdateAsync(user, ct);

        return Result.Success(Unit.Value);
    }
}
