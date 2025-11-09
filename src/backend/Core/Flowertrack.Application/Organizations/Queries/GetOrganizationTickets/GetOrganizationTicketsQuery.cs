using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Organizations.Queries.GetOrganizationTickets;

/// <summary>
/// Query to get tickets for an organization
/// </summary>
public sealed record GetOrganizationTicketsQuery(Guid OrganizationId) : IRequest<Result<List<TicketSummaryDto>>>;
