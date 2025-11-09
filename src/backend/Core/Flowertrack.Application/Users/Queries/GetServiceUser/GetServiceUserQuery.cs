using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Queries.GetServiceUser;

public record GetServiceUserQuery(Guid Id) : IRequest<Result<ServiceUserDetailsDto>>;
