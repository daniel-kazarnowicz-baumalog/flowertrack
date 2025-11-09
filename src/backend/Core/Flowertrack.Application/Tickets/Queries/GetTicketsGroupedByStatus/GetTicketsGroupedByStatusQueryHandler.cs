using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Queries.GetTicketsGroupedByStatus;

/// <summary>
/// Handler for GetTicketsGroupedByStatusQuery - groups tickets by status with counts and samples
/// </summary>
public sealed class GetTicketsGroupedByStatusQueryHandler 
    : IRequestHandler<GetTicketsGroupedByStatusQuery, Result<List<TicketStatusGroupDto>>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly ILogger<GetTicketsGroupedByStatusQueryHandler> _logger;

    public GetTicketsGroupedByStatusQueryHandler(
        ITicketRepository ticketRepository,
        IOrganizationRepository organizationRepository,
        IMachineRepository machineRepository,
        IOrganizationUserRepository organizationUserRepository,
        ILogger<GetTicketsGroupedByStatusQueryHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _organizationRepository = organizationRepository;
        _machineRepository = machineRepository;
        _organizationUserRepository = organizationUserRepository;
        _logger = logger;
    }

    public async Task<Result<List<TicketStatusGroupDto>>> Handle(
        GetTicketsGroupedByStatusQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Grouping tickets by status for user {UserId}", 
                request.RequestedBy);

            // Validate sample size
            var sampleSize = request.SampleSize;
            if (sampleSize < 1) sampleSize = 5;
            if (sampleSize > 20) sampleSize = 20;

            // Get user's organization to enforce access control
            var userOrganization = await _organizationUserRepository.GetByIdAsync(
                request.RequestedBy, 
                cancellationToken);

            if (userOrganization == null)
            {
                _logger.LogWarning(
                    "User {UserId} not found in any organization", 
                    request.RequestedBy);
                return Result.Failure<List<TicketStatusGroupDto>>(
                    "User not found in any organization");
            }

            // Get all tickets for user's organization
            var allTickets = await _ticketRepository.GetAllAsync(cancellationToken);
            
            var organizationId = request.OrganizationId ?? userOrganization.OrganizationId;
            
            var tickets = allTickets
                .Where(t => !t.IsDeleted && t.OrganizationId == organizationId)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            if (tickets.Count == 0)
            {
                _logger.LogInformation("No tickets found for organization {OrgId}", organizationId);
                return Result.Success(new List<TicketStatusGroupDto>());
            }

            // Group by status
            var groupedTickets = tickets
                .GroupBy(t => t.Status)
                .OrderBy(g => (int)g.Key) // Order by status enum value
                .ToList();

            // Collect unique organization and machine IDs for batch loading
            var organizationIds = tickets.Select(t => t.OrganizationId).Distinct().ToList();
            var machineIds = tickets.Select(t => t.MachineId).Distinct().ToList();

            // Batch load related entities
            var organizations = await Task.WhenAll(
                organizationIds.Select(id => _organizationRepository.GetByIdAsync(id, cancellationToken)));

            var machines = await Task.WhenAll(
                machineIds.Select(id => _machineRepository.GetByIdAsync(id, cancellationToken)));

            // Create dictionaries for fast lookup
            var orgDict = organizations.Where(o => o != null).ToDictionary(o => o!.Id, o => o);
            var machineDict = machines.Where(m => m != null).ToDictionary(m => m!.Id, m => m);

            // Build result
            var result = groupedTickets.Select(group =>
            {
                var sampleTickets = group
                    .Take(sampleSize)
                    .Select(ticket =>
                    {
                        var org = orgDict.GetValueOrDefault(ticket.OrganizationId);
                        var machine = machineDict.GetValueOrDefault(ticket.MachineId);

                        return new TicketSampleDto
                        {
                            Id = ticket.Id,
                            TicketNumber = ticket.TicketNumber.Value,
                            Title = ticket.Title,
                            Priority = ticket.Priority,
                            OrganizationName = org?.Name ?? "Unknown",
                            MachineSerialNumber = machine?.SerialNumber,
                            CreatedAt = ticket.CreatedAt
                        };
                    })
                    .ToList();

                return new TicketStatusGroupDto
                {
                    Status = group.Key,
                    Count = group.Count(),
                    SampleTickets = sampleTickets
                };
            }).ToList();

            _logger.LogInformation(
                "Grouped {TotalTickets} tickets into {GroupCount} status groups for organization {OrgId}",
                tickets.Count,
                result.Count,
                organizationId);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error grouping tickets by status");
            return Result.Failure<List<TicketStatusGroupDto>>(
                "An error occurred while grouping tickets by status");
        }
    }
}
