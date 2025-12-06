using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Dashboard.Queries.GetOrganizationActivities;

/// <summary>
/// Handler for GetOrganizationActivitiesQuery
/// US-038: Ostatnie aktywności (Portal Klienta)
/// </summary>
public sealed class GetOrganizationActivitiesQueryHandler
    : IRequestHandler<GetOrganizationActivitiesQuery, Result<OrganizationActivitiesDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<GetOrganizationActivitiesQueryHandler> _logger;

    public GetOrganizationActivitiesQueryHandler(
        IApplicationDbContext context,
        IOrganizationRepository organizationRepository,
        ILogger<GetOrganizationActivitiesQueryHandler> logger)
    {
        _context = context;
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    public async Task<Result<OrganizationActivitiesDto>> Handle(
        GetOrganizationActivitiesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Fetching organization activities, Organization: {OrganizationId}, Limit: {Limit}, User: {UserId}",
                request.OrganizationId,
                request.Limit,
                request.RequestedBy);

            // Validate organization exists
            var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
            if (organization == null)
            {
                return Result.Failure<OrganizationActivitiesDto>("Organization not found");
            }

            var limit = Math.Max(1, Math.Min(request.Limit, 100)); // Max 100 items
            var activities = new List<RecentActivityDto>();

            // Get recent ticket creations for this organization
            var recentTickets = await _context.Tickets
                .Where(t => !t.IsDeleted && t.OrganizationId == request.OrganizationId)
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
                    OrganizationName = organization.Name,
                    Timestamp = ticket.CreatedAt,
                    PerformedBy = "User"
                });
            }

            // Get recent comments for this organization's tickets (excluding internal notes)
            var recentComments = await _context.TicketComments
                .Include(c => c.Ticket)
                .Where(c => !c.IsDeleted && !c.IsInternal) // Only public comments for client portal
                .Where(c => c.Ticket!.OrganizationId == request.OrganizationId)
                .OrderByDescending(c => c.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);

            foreach (var comment in recentComments)
            {
                activities.Add(new RecentActivityDto
                {
                    Id = comment.Id,
                    ActivityType = "CommentAdded",
                    Description = $"Comment added to ticket",
                    RelatedEntity = comment.Ticket?.TicketNumber.Value ?? "Unknown",
                    OrganizationName = organization.Name,
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
                "Retrieved {Count} activities for organization {OrganizationId}",
                sortedActivities.Count,
                request.OrganizationId);

            return Result.Success(new OrganizationActivitiesDto
            {
                OrganizationId = request.OrganizationId,
                OrganizationName = organization.Name,
                Activities = sortedActivities
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching organization activities");
            return Result.Failure<OrganizationActivitiesDto>($"Failed to fetch organization activities: {ex.Message}");
        }
    }
}
