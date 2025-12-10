using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Queries.GetTicket;

/// <summary>
/// Handler for GetTicketQuery
/// Retrieves ticket with authorization check (user must be from same organization or service team)
/// </summary>
public sealed class GetTicketQueryHandler
    : IRequestHandler<GetTicketQuery, Result<TicketDetailDto>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IOrganizationUserRepository _userRepository;
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly ILogger<GetTicketQueryHandler> _logger;

    public GetTicketQueryHandler(
        ITicketRepository ticketRepository,
        IOrganizationRepository organizationRepository,
        IMachineRepository machineRepository,
        IOrganizationUserRepository userRepository,
        IServiceUserRepository serviceUserRepository,
        ILogger<GetTicketQueryHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _organizationRepository = organizationRepository;
        _machineRepository = machineRepository;
        _userRepository = userRepository;
        _serviceUserRepository = serviceUserRepository;
        _logger = logger;
    }

    public async Task<Result<TicketDetailDto>> Handle(
        GetTicketQuery request,
        CancellationToken cancellationToken)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket == null)
        {
            _logger.LogWarning("Ticket {TicketId} not found", request.TicketId);
            return Result.Failure<TicketDetailDto>("Ticket not found");
        }

        // Check if user is a service team member (has access to all tickets)
        var serviceUser = await _serviceUserRepository.GetByIdAsync(request.RequestedBy, cancellationToken);
        var isServiceUser = serviceUser != null;

        // If not a service user, check organization user authorization
        if (!isServiceUser)
        {
            var requestingUser = await _userRepository.GetByIdAsync(request.RequestedBy, cancellationToken);
            if (requestingUser == null)
            {
                _logger.LogWarning("Requesting user {UserId} not found", request.RequestedBy);
                return Result.Failure<TicketDetailDto>("User not found");
            }

            // Authorization check: User must belong to the same organization
            if (requestingUser.OrganizationId != ticket.OrganizationId)
            {
                _logger.LogWarning(
                    "User {UserId} from organization {UserOrgId} attempted to access ticket {TicketId} from organization {TicketOrgId}",
                    request.RequestedBy,
                    requestingUser.OrganizationId,
                    request.TicketId,
                    ticket.OrganizationId);
                return Result.Failure<TicketDetailDto>("You do not have permission to view this ticket");
            }
        }

        // Load related entities
        var organization = await _organizationRepository.GetByIdAsync(ticket.OrganizationId, cancellationToken);
        var machine = await _machineRepository.GetByIdAsync(ticket.MachineId, cancellationToken);
        var createdByUser = await _userRepository.GetByIdAsync(ticket.CreatedByUserId, cancellationToken);
        
        string? assignedToUserName = null;
        if (ticket.AssignedToUserId.HasValue)
        {
            // Try to find assigned user in service users first, then organization users
            var assignedServiceUser = await _serviceUserRepository.GetByIdAsync(ticket.AssignedToUserId.Value, cancellationToken);
            if (assignedServiceUser != null)
            {
                assignedToUserName = $"{assignedServiceUser.FirstName} {assignedServiceUser.LastName}";
            }
            else
            {
                var assignedUser = await _userRepository.GetByIdAsync(ticket.AssignedToUserId.Value, cancellationToken);
                assignedToUserName = assignedUser != null 
                    ? $"{assignedUser.FirstName} {assignedUser.LastName}" 
                    : null;
            }
        }

        // Map to DTO
        var dto = new TicketDetailDto
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber.Value,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status.ToString(),
            Priority = ticket.Priority.ToString(),
            OrganizationId = ticket.OrganizationId,
            OrganizationName = organization?.Name ?? "Unknown",
            MachineId = ticket.MachineId,
            MachineSerialNumber = machine?.SerialNumber ?? "Unknown",
            MachineBrand = machine?.Brand ?? "Unknown",
            MachineModel = machine?.Model ?? "Unknown",
            CreatedByUserId = ticket.CreatedByUserId,
            CreatedByUserName = createdByUser != null 
                ? $"{createdByUser.FirstName} {createdByUser.LastName}" 
                : "Unknown",
            AssignedToUserId = ticket.AssignedToUserId,
            AssignedToUserName = assignedToUserName,
            ResolvedAt = ticket.ResolvedAt,
            ClosedAt = ticket.ClosedAt,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            UpdatedBy = ticket.UpdatedBy
        };

        _logger.LogInformation(
            "Successfully retrieved ticket {TicketId} for user {UserId}",
            request.TicketId,
            request.RequestedBy);

        return Result.Success(dto);
    }
}
