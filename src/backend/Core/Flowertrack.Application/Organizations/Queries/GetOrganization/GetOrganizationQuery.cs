using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Organizations.Queries.GetOrganization;

/// <summary>
/// Query to get organization details
/// </summary>
public sealed record GetOrganizationQuery(Guid OrganizationId) : IRequest<Result<OrganizationDetailsDto>>;
