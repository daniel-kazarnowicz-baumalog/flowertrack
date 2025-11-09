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
    private readonly ILogger<GetTicketsQueryHandler> _logger;

    public GetTicketsQueryHandler(
        ITicketRepository ticketRepository,
        IOrganizationRepository organizationRepository,
        IMachineRepository machineRepository,
        IOrganizationUserRepository organizationUserRepository,
        ILogger<GetTicketsQueryHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _organizationRepository = organizationRepository;
        _machineRepository = machineRepository;
        _organizationUserRepository = organizationUserRepository;
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

            // Get user's organizations to determine access
            var userOrganizations = await _organizationUserRepository.GetByIdAsync(
                request.RequestedBy,
                cancellationToken);

            if (userOrganizations == null)
            {
                _logger.LogWarning("User {UserId} not found", request.RequestedBy);
                return Result.Failure<PagedResult<TicketListItemDto>>("User not found");
            }

            // Get all tickets (filtered by access)
            var allTickets = await _ticketRepository.GetAllAsync(cancellationToken);

            // Filter by organization access (user can only see tickets from their organization)
            var query = allTickets
                .Where(t => !t.IsDeleted && t.OrganizationId == userOrganizations.OrganizationId)
                .AsQueryable();

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

            // Load related data
            var organizationIds = tickets.Select(t => t.OrganizationId).Distinct().ToList();
            var machineIds = tickets.Select(t => t.MachineId).Distinct().ToList();
            var userIds = tickets
                .SelectMany(t => new[] { t.CreatedByUserId, t.AssignedToUserId })
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var organizations = await Task.WhenAll(
                organizationIds.Select(id => _organizationRepository.GetByIdAsync(id, cancellationToken)));

            var machines = await Task.WhenAll(
                machineIds.Select(id => _machineRepository.GetByIdAsync(id, cancellationToken)));

            var users = await Task.WhenAll(
                userIds.Select(id => _organizationUserRepository.GetByIdAsync(id, cancellationToken)));

            // Create dictionaries for fast lookup
            var orgDict = organizations.Where(o => o != null).ToDictionary(o => o!.Id, o => o);
            var machineDict = machines.Where(m => m != null).ToDictionary(m => m!.Id, m => m);
            var userDict = users.Where(u => u != null).ToDictionary(u => u!.Id, u => u);

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
