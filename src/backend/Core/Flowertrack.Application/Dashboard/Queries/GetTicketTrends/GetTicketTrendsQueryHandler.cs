using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Dashboard.Queries.GetTicketTrends;

/// <summary>
/// Handler for GetTicketTrendsQuery
/// US-007: Wykres trendów zgłoszeń
/// </summary>
public sealed class GetTicketTrendsQueryHandler
    : IRequestHandler<GetTicketTrendsQuery, Result<TicketTrendsDto>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ILogger<GetTicketTrendsQueryHandler> _logger;

    public GetTicketTrendsQueryHandler(
        ITicketRepository ticketRepository,
        ILogger<GetTicketTrendsQueryHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _logger = logger;
    }

    public async Task<Result<TicketTrendsDto>> Handle(
        GetTicketTrendsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Generating ticket trends for {Days} days, Organization: {OrganizationId}, User: {UserId}",
                request.Days,
                request.OrganizationId,
                request.RequestedBy);

            // Validate days range
            var days = Math.Max(1, Math.Min(request.Days, 90)); // Max 90 days

            var endDate = DateTimeOffset.UtcNow.Date.AddDays(1); // End of today
            var startDate = endDate.AddDays(-days);

            // Get all tickets in the date range
            var allTickets = await _ticketRepository.GetAllAsync(cancellationToken);
            var tickets = allTickets
                .Where(t => !t.IsDeleted)
                .Where(t => request.OrganizationId == null || t.OrganizationId == request.OrganizationId)
                .ToList();

            // Build daily data
            var dates = new List<string>();
            var opened = new List<int>();
            var resolved = new List<int>();
            var closed = new List<int>();

            for (var date = startDate; date < endDate; date = date.AddDays(1))
            {
                var nextDate = date.AddDays(1);
                var dateStr = date.ToString("yyyy-MM-dd");

                dates.Add(dateStr);

                // Tickets created on this date
                var openedCount = tickets.Count(t =>
                    t.CreatedAt >= date && t.CreatedAt < nextDate);
                opened.Add(openedCount);

                // Tickets resolved on this date
                var resolvedCount = tickets.Count(t =>
                    t.ResolvedAt.HasValue &&
                    t.ResolvedAt.Value >= date &&
                    t.ResolvedAt.Value < nextDate);
                resolved.Add(resolvedCount);

                // Tickets closed on this date
                var closedCount = tickets.Count(t =>
                    t.ClosedAt.HasValue &&
                    t.ClosedAt.Value >= date &&
                    t.ClosedAt.Value < nextDate);
                closed.Add(closedCount);
            }

            // Priority breakdown for active tickets in the period
            var activeStatuses = new[] { TicketStatus.New, TicketStatus.Accepted, TicketStatus.InProgress, TicketStatus.Reopened };
            var activeTickets = tickets.Where(t => activeStatuses.Contains(t.Status)).ToList();

            var byPriority = new PriorityBreakdownDto
            {
                Critical = activeTickets.Count(t => t.Priority == Priority.Critical),
                High = activeTickets.Count(t => t.Priority == Priority.High),
                Medium = activeTickets.Count(t => t.Priority == Priority.Medium),
                Low = activeTickets.Count(t => t.Priority == Priority.Low)
            };

            var result = new TicketTrendsDto
            {
                Dates = dates,
                Opened = opened,
                Resolved = resolved,
                Closed = closed,
                ByPriority = byPriority
            };

            _logger.LogInformation(
                "Generated ticket trends: {TotalDays} days, {TotalOpened} opened, {TotalResolved} resolved",
                dates.Count,
                opened.Sum(),
                resolved.Sum());

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating ticket trends");
            return Result.Failure<TicketTrendsDto>($"Failed to generate ticket trends: {ex.Message}");
        }
    }
}
