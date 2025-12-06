using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Queries.GetServiceUserStats;

/// <summary>
/// Query to get statistics for a service user.
/// US-031: Statystyki serwisanta
/// </summary>
public record GetServiceUserStatsQuery(Guid UserId) : IRequest<Result<ServiceUserStatsDto>>;
