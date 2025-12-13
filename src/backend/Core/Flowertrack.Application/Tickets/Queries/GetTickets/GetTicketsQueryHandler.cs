using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Queries.GetTickets;

/// <summary>
/// Handler for GetTicketsQuery with filtering, pagination, and sorting
/// </summary>
public sealed class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, Result<PagedResult<TicketListItemDto>>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly ILogger<GetTicketsQueryHandler> _logger;

    public GetTicketsQueryHandler(
        ITicketRepository ticketRepository,
        IOrganizationRepository organizationRepository,
        IMachineRepository machineRepository,
        IOrganizationUserRepository organizationUserRepository,
        IServiceUserRepository serviceUserRepository,
        ILogger<GetTicketsQueryHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _organizationRepository = organizationRepository;
        _machineRepository = machineRepository;
        _organizationUserRepository = organizationUserRepository;
        _serviceUserRepository = serviceUserRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResult<TicketListItemDto>>> Handle(
        GetTicketsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Retrieving tickets for user {UserId} with filters: Org={OrgId}, Status={Status}, Page={Page}",
                request.RequestedBy,
                request.OrganizationId,
                request.Status,
                request.PageNumber);

            // Validate page size
            var pageSize = Math.Min(request.PageSize, 100);
            if (pageSize < 1) pageSize = 20;

            var pageNumber = Math.Max(request.PageNumber, 1);

            // Check if user is a Service User
            var serviceUser = await _serviceUserRepository.GetByIdAsync(request.RequestedBy, cancellationToken);
            Guid? userOrganizationId = null;

            if (serviceUser == null)
            {
                // If not service user, check organization user
                var userOrganizations = await _organizationUserRepository.GetByIdAsync(
                    request.RequestedBy,
                    cancellationToken);

                if (userOrganizations == null)
                {
                    _logger.LogWarning("User {UserId} not found", request.RequestedBy);
                    return Result.Failure<PagedResult<TicketListItemDto>>("User not found");
                }

                userOrganizationId = userOrganizations.OrganizationId;
            }

            // Get all tickets (filtered by access)
            var allTickets = await _ticketRepository.GetAllAsync(cancellationToken);

            var query = allTickets.AsQueryable();

            // Filter by organization access (user can only see tickets from their organization)
            if (userOrganizationId.HasValue)
            {
                query = query.Where(t => !t.IsDeleted && t.OrganizationId == userOrganizationId.Value);
            }
            else
            {
                // Service users see all non-deleted tickets
                query = query.Where(t => !t.IsDeleted);
            }

            // Apply filters
            if (request.OrganizationId.HasValue)
            {
                query = query.Where(t => t.OrganizationId == request.OrganizationId.Value);
            }

            if (request.MachineId.HasValue)
            {
                query = query.Where(t => t.MachineId == request.MachineId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(t => t.Status == request.Status.Value);
            }

            if (request.Priority.HasValue)
            {
                query = query.Where(t => t.Priority == request.Priority.Value);
            }

            if (request.AssignedToUserId.HasValue)
            {
                query = query.Where(t => t.AssignedToUserId == request.AssignedToUserId.Value);
            }

            if (request.CreatedByUserId.HasValue)
            {
                query = query.Where(t => t.CreatedByUserId == request.CreatedByUserId.Value);
            }

            if (request.CreatedFrom.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= request.CreatedFrom.Value);
            }

            if (request.CreatedTo.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= request.CreatedTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var searchLower = request.SearchText.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(searchLower) ||
                    t.Description.ToLower().Contains(searchLower) ||
                    t.TicketNumber.Value.ToLower().Contains(searchLower));
            }

            // Get total count before pagination
            var totalCount = query.Count();

            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            // Apply pagination
            var tickets = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Load related data sequentially (DbContext is not thread-safe)
            var organizationIds = tickets.Select(t => t.OrganizationId).Distinct().ToList();
            var machineIds = tickets.Select(t => t.MachineId).Distinct().ToList();
            var userIds = tickets
                .SelectMany(t => new[] { t.CreatedByUserId, t.AssignedToUserId })
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            // Load organizations sequentially
            var orgDict = new Dictionary<Guid, Domain.Entities.Organization>();
            foreach (var id in organizationIds)
            {
                var org = await _organizationRepository.GetByIdAsync(id, cancellationToken);
                if (org != null)
                {
                    orgDict[org.Id] = org;
                }
            }

            // Load machines sequentially
            var machineDict = new Dictionary<Guid, Domain.Entities.Machine>();
            foreach (var id in machineIds)
            {
                var machine = await _machineRepository.GetByIdAsync(id, cancellationToken);
                if (machine != null)
                {
                    machineDict[machine.Id] = machine;
                }
            }

            // Load users sequentially
            var userDict = new Dictionary<Guid, Domain.Entities.OrganizationUser>();
            foreach (var id in userIds)
            {
                var user = await _organizationUserRepository.GetByIdAsync(id, cancellationToken);
                if (user != null)
                {
                    userDict[user.Id] = user;
                }
            }

            // Map to DTOs
            var items = tickets.Select(ticket =>
            {
                var org = orgDict.GetValueOrDefault(ticket.OrganizationId);
                var machine = machineDict.GetValueOrDefault(ticket.MachineId);
                var createdBy = userDict.GetValueOrDefault(ticket.CreatedByUserId);
                var assignedTo = ticket.AssignedToUserId.HasValue
                    ? userDict.GetValueOrDefault(ticket.AssignedToUserId.Value)
                    : null;

                return new TicketListItemDto
                {
                    Id = ticket.Id,
                    TicketNumber = ticket.TicketNumber.Value,
                    Title = ticket.Title,
                    Status = ticket.Status,
                    Priority = ticket.Priority,
                    OrganizationId = ticket.OrganizationId,
                    OrganizationName = org?.Name ?? "Unknown",
                    MachineId = ticket.MachineId,
                    MachineSerialNumber = machine?.SerialNumber,
                    AssignedToUserId = ticket.AssignedToUserId,
                    AssignedToUserName = assignedTo != null
                        ? $"{assignedTo.FirstName} {assignedTo.LastName}"
                        : null,
                    CreatedByUserId = ticket.CreatedByUserId,
                    CreatedByUserName = createdBy != null
                        ? $"{createdBy.FirstName} {createdBy.LastName}"
                        : "Unknown",
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt,
                    ResolvedAt = ticket.ResolvedAt,
                    ClosedAt = ticket.ClosedAt
                };
            }).ToList();

            var pagedResult = new PagedResult<TicketListItemDto>(
                items,
                totalCount,
                pageNumber,
                pageSize);

            _logger.LogInformation(
                "Retrieved {Count} tickets (page {Page}/{TotalPages})",
                items.Count,
                pageNumber,
                pagedResult.TotalPages);

            return Result.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tickets");
            return Result.Failure<PagedResult<TicketListItemDto>>(
                $"Failed to retrieve tickets: {ex.Message}");
        }
    }

    private static IQueryable<Domain.Entities.Ticket> ApplySorting(
        IQueryable<Domain.Entities.Ticket> query,
        string sortBy,
        string sortDirection)
    {
        var isDescending = sortDirection.Equals("Desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLower() switch
        {
            "ticketnumber" => isDescending
                ? query.OrderByDescending(t => t.TicketNumber.Value)
                : query.OrderBy(t => t.TicketNumber.Value),
            "title" => isDescending
                ? query.OrderByDescending(t => t.Title)
                : query.OrderBy(t => t.Title),
            "status" => isDescending
                ? query.OrderByDescending(t => t.Status)
                : query.OrderBy(t => t.Status),
            "priority" => isDescending
                ? query.OrderByDescending(t => t.Priority)
                : query.OrderBy(t => t.Priority),
            "updatedat" => isDescending
                ? query.OrderByDescending(t => t.UpdatedAt)
                : query.OrderBy(t => t.UpdatedAt),
            "createdat" or _ => isDescending
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt),
        };
    }
}
