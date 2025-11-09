using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Organizations.Queries.GetOrganizationUsers;

/// <summary>
/// Query to get users for an organization
/// </summary>
public sealed record GetOrganizationUsersQuery(Guid OrganizationId) : IRequest<Result<List<OrganizationUserSummaryDto>>>;
