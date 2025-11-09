using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.RemoveOrganizationUser;

public record RemoveOrganizationUserCommand(
    Guid UserId,
    Guid OrganizationId) : IRequest<Result<Unit>>;
