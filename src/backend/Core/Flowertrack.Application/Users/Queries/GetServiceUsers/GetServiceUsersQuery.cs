using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Queries.GetServiceUsers;

public record GetServiceUsersQuery(
    string? Status,
    string? Role,
    string? SearchTerm,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<List<ServiceUserDto>>>;
