using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Auth.Queries.GetCurrentServiceUser;

/// <summary>
/// Query to get the current authenticated service user
/// </summary>
public sealed record GetCurrentServiceUserQuery(Guid UserId) : IRequest<Result<ServiceUserDto>>;
