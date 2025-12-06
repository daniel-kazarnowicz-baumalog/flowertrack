using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Dashboard.Queries.GetRecentActivities;

/// <summary>
/// Handler for GetRecentActivitiesQuery
/// US-008: Lista ostatnich zdarzeń
/// </summary>
public sealed class GetRecentActivitiesQueryHandler
    : IRequestHandler<GetRecentActivitiesQuery, Result<RecentActivitiesDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<GetRecentActivitiesQueryHandler> _logger;

    public GetRecentActivitiesQueryHandler(
        IApplicationDbContext context,
        IOrganizationRepository organizationRepository,
        ILogger<GetRecentActivitiesQueryHandler> logger)
    {
        _context = context;
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    public async Task<Result<RecentActivitiesDto>> Handle(
        GetRecentActivitiesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Fetching recent activities, Limit: {Limit}, Organization: {OrganizationId}, User: {UserId}",
                request.Limit,
                request.OrganizationId,
                request.RequestedBy);

            var limit = Math.Max(1, Math.Min(request.Limit, 100)); // Max 100 items

            // Build activities from multiple sources
            var activities = new List<RecentActivityDto>();

            // Get organization names lookup
            var organizations = await _organizationRepository.GetAllAsync(cancellationToken);
            var orgDict = organizations.ToDictionary(o => o.Id, o => o.Name);

            // Get recent ticket creations
            var recentTickets = await _context.Tickets
                .Where(t => !t.IsDeleted)
                .Where(t => request.OrganizationId == null || t.OrganizationId == request.OrganizationId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);

            foreach (var ticket in recentTickets)
            {
                activities.Add(new RecentActivityDto
                {
                    Id = ticket.Id,
                    ActivityType = "TicketCreated",
                    Description = $"Ticket '{ticket.Title}' was created",
                    RelatedEntity = ticket.TicketNumber.Value,
                    OrganizationName = orgDict.GetValueOrDefault(ticket.OrganizationId, "Unknown"),
                    Timestamp = ticket.CreatedAt,
                    PerformedBy = "User"
                });
            }

            // Get recent comments
            var recentComments = await _context.TicketComments
                .Include(c => c.Ticket)
                .Where(c => !c.IsDeleted)
                .Where(c => request.OrganizationId == null || c.Ticket!.OrganizationId == request.OrganizationId)
                .OrderByDescending(c => c.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);

            foreach (var comment in recentComments)
            {
                activities.Add(new RecentActivityDto
                {
                    Id = comment.Id,
                    ActivityType = comment.IsInternal ? "NoteAdded" : "CommentAdded",
                    Description = comment.IsInternal 
                        ? $"Internal note added to ticket" 
                        : $"Comment added to ticket",
                    RelatedEntity = comment.Ticket?.TicketNumber.Value ?? "Unknown",
                    OrganizationName = orgDict.GetValueOrDefault(comment.Ticket?.OrganizationId ?? Guid.Empty, "Unknown"),
                    Timestamp = comment.CreatedAt,
                    PerformedBy = "User" // UserId: comment.UserId
                });
            }

            // Sort all activities by timestamp and take the requested limit
            var sortedActivities = activities
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToList();

            _logger.LogInformation(
                "Retrieved {Count} recent activities",
                sortedActivities.Count);

            return Result.Success(new RecentActivitiesDto
            {
                Activities = sortedActivities
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent activities");
            return Result.Failure<RecentActivitiesDto>($"Failed to fetch recent activities: {ex.Message}");
        }
    }
}
