using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Organizations.Queries.GetOrganizationTickets;

/// <summary>
/// Handler for GetOrganizationTicketsQuery
/// </summary>
public sealed class GetOrganizationTicketsQueryHandler 
    : IRequestHandler<GetOrganizationTicketsQuery, Result<List<TicketSummaryDto>>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public GetOrganizationTicketsQueryHandler(
        ITicketRepository ticketRepository,
        IOrganizationRepository organizationRepository)
    {
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
    }

    public async Task<Result<List<TicketSummaryDto>>> Handle(
        GetOrganizationTicketsQuery request, 
        CancellationToken cancellationToken)
    {
        // Verify organization exists
        var organizationExists = await _organizationRepository.ExistsAsync(request.OrganizationId, cancellationToken);
        if (!organizationExists)
        {
            return Result.Failure<List<TicketSummaryDto>>(
                $"Organization with ID {request.OrganizationId} was not found");
        }

        // Get tickets
        var tickets = await _ticketRepository.GetByOrganizationIdAsync(request.OrganizationId, cancellationToken);

        var ticketDtos = tickets.Select(t => new TicketSummaryDto
        {
            Id = t.Id,
            TicketNumber = t.TicketNumber.Value,
            Title = t.Title,
            Status = t.Status.ToString(),
            Priority = t.Priority.ToString(),
            MachineId = t.MachineId,
            AssignedToUserId = t.AssignedToUserId,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();

        return Result.Success(ticketDtos);
    }
}
