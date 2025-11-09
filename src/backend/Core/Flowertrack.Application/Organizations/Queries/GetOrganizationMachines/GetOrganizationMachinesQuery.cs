using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Organizations.Queries.GetOrganizationMachines;

/// <summary>
/// Query to get machines for an organization
/// </summary>
public sealed record GetOrganizationMachinesQuery(Guid OrganizationId) : IRequest<Result<List<MachineSummaryDto>>>;
