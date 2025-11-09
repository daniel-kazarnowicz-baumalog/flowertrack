using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Auth.Queries.GetCurrentOrganizationUser;

/// <summary>
/// Query to get the current authenticated organization user
/// </summary>
public sealed record GetCurrentOrganizationUserQuery(Guid UserId) : IRequest<Result<OrganizationUserDto>>;
