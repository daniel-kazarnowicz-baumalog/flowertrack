using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Queries.GetOrganizationUsers;

public record GetOrganizationUsersQuery(
    Guid OrganizationId,
    string? Status,
    string? Role,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<List<OrganizationUserDto>>>;
