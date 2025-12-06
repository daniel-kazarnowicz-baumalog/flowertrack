using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Users.Queries.GetServiceUserStats;

/// <summary>
/// Handler for GetServiceUserStatsQuery.
/// US-031: Calculates and returns service user statistics.
/// </summary>
public class GetServiceUserStatsQueryHandler : IRequestHandler<GetServiceUserStatsQuery, Result<ServiceUserStatsDto>>
{
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly ITicketRepository _ticketRepository;

    public GetServiceUserStatsQueryHandler(
        IServiceUserRepository serviceUserRepository,
        ITicketRepository ticketRepository)
    {
        _serviceUserRepository = serviceUserRepository;
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<ServiceUserStatsDto>> Handle(
        GetServiceUserStatsQuery request,
        CancellationToken ct)
    {
        // Verify user exists
        var user = await _serviceUserRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
        {
            return Result.Failure<ServiceUserStatsDto>("Service user not found");
        }

        // Get all tickets assigned to this user
        var allTickets = await _ticketRepository.GetByAssignedUserIdAsync(request.UserId, ct);

        var now = DateTimeOffset.UtcNow;
        var thirtyDaysAgo = now.AddDays(-30);

        // Calculate statistics
        var ticketsInProgress = allTickets.Count(t =>
            t.Status == TicketStatus.InProgress ||
            t.Status == TicketStatus.Accepted);

        var ticketsResolvedLast30Days = allTickets.Count(t =>
            t.Status == TicketStatus.Resolved &&
            t.ResolvedAt.HasValue &&
            t.ResolvedAt.Value >= thirtyDaysAgo);

        var ticketsClosedLast30Days = allTickets.Count(t =>
            t.Status == TicketStatus.Closed &&
            t.ClosedAt.HasValue &&
            t.ClosedAt.Value >= thirtyDaysAgo);

        // Calculate average resolution time for resolved tickets (from creation to resolution)
        var resolvedTickets = allTickets
            .Where(t => t.ResolvedAt.HasValue)
            .ToList();

        double? avgResolutionTimeHours = null;
        if (resolvedTickets.Count > 0)
        {
            var totalHours = resolvedTickets
                .Select(t => (t.ResolvedAt!.Value - t.CreatedAt).TotalHours)
                .Sum();
            avgResolutionTimeHours = Math.Round(totalHours / resolvedTickets.Count, 2);
        }

        // Find last activity (most recent ResolvedAt or UpdatedAt)
        var lastActivity = allTickets
            .Select(t => t.ResolvedAt ?? t.UpdatedAt ?? t.CreatedAt)
            .OrderByDescending(d => d)
            .FirstOrDefault();

        // Priority breakdown for active tickets
        var activeTickets = allTickets.Where(t =>
            t.Status != TicketStatus.Closed &&
            t.Status != TicketStatus.Resolved).ToList();

        var priorityBreakdown = new TicketsByPriorityDto
        {
            Critical = activeTickets.Count(t => t.Priority == Priority.Critical),
            High = activeTickets.Count(t => t.Priority == Priority.High),
            Medium = activeTickets.Count(t => t.Priority == Priority.Medium),
            Low = activeTickets.Count(t => t.Priority == Priority.Low)
        };

        var stats = new ServiceUserStatsDto
        {
            UserId = request.UserId,
            TotalTicketsAssigned = allTickets.Count,
            TicketsInProgress = ticketsInProgress,
            TicketsResolvedLast30Days = ticketsResolvedLast30Days,
            TicketsClosedLast30Days = ticketsClosedLast30Days,
            AverageResolutionTimeHours = avgResolutionTimeHours,
            LastActivityAt = lastActivity != default ? lastActivity : null,
            TicketsByPriority = priorityBreakdown
        };

        return Result.Success(stats);
    }
}
