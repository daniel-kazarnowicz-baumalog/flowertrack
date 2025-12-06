using System.Text;
using System.Text.Json;
using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Tickets.Queries.GetTicketHistory;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Queries.ExportTicketHistory;

/// <summary>
/// Handler for ExportTicketHistoryQuery
/// US-022: Eksport historii ticketu
/// </summary>
public sealed class ExportTicketHistoryQueryHandler
    : IRequestHandler<ExportTicketHistoryQuery, Result<ExportResult>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ExportTicketHistoryQueryHandler> _logger;

    public ExportTicketHistoryQueryHandler(
        ITicketRepository ticketRepository,
        IApplicationDbContext context,
        ILogger<ExportTicketHistoryQueryHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<ExportResult>> Handle(
        ExportTicketHistoryQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Exporting ticket history for {TicketId} in format {Format} by user {UserId}",
                request.TicketId,
                request.Format,
                request.RequestedBy);

            // Get ticket with validation
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);
            if (ticket == null || ticket.IsDeleted)
            {
                return Result.Failure<ExportResult>("Ticket not found");
            }

            // Get ticket history (comments)
            var comments = await _context.TicketComments
                .Where(c => c.TicketId == request.TicketId && !c.IsDeleted)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            // Build export data
            var exportData = new TicketExportData
            {
                TicketNumber = ticket.TicketNumber.Value,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Priority = ticket.Priority.ToString(),
                CreatedAt = ticket.CreatedAt,
                ResolvedAt = ticket.ResolvedAt,
                ClosedAt = ticket.ClosedAt,
                History = comments.Select(c => new HistoryExportItem
                {
                    Type = c.IsInternal ? "Note" : "Comment",
                    Content = c.Content,
                    UserName = c.UserId.ToString(), // TODO: Resolve user name from UserId
                    CreatedAt = c.CreatedAt
                }).ToList()
            };

            var result = request.Format switch
            {
                ExportFormat.Json => ExportToJson(exportData, ticket.TicketNumber.Value),
                ExportFormat.Csv => ExportToCsv(exportData, ticket.TicketNumber.Value),
                ExportFormat.Pdf => ExportToPdf(exportData, ticket.TicketNumber.Value),
                _ => ExportToJson(exportData, ticket.TicketNumber.Value)
            };

            _logger.LogInformation(
                "Successfully exported ticket {TicketNumber} history as {Format}",
                ticket.TicketNumber.Value,
                request.Format);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting ticket history for {TicketId}", request.TicketId);
            return Result.Failure<ExportResult>($"Failed to export ticket history: {ex.Message}");
        }
    }

    private static ExportResult ExportToJson(TicketExportData data, string ticketNumber)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return new ExportResult
        {
            Content = Encoding.UTF8.GetBytes(json),
            ContentType = "application/json",
            FileName = $"ticket-{ticketNumber}-history.json"
        };
    }

    private static ExportResult ExportToCsv(TicketExportData data, string ticketNumber)
    {
        var sb = new StringBuilder();

        // Header row
        sb.AppendLine("Ticket Number,Title,Status,Priority,Created At,Resolved At,Closed At");

        // Ticket info row
        sb.AppendLine($"\"{data.TicketNumber}\",\"{EscapeCsv(data.Title)}\",\"{data.Status}\",\"{data.Priority}\",\"{data.CreatedAt:yyyy-MM-dd HH:mm:ss}\",\"{data.ResolvedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}\",\"{data.ClosedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}\"");

        sb.AppendLine();
        sb.AppendLine("History");
        sb.AppendLine("Type,Content,User,Created At");

        foreach (var item in data.History)
        {
            sb.AppendLine($"\"{item.Type}\",\"{EscapeCsv(item.Content)}\",\"{EscapeCsv(item.UserName)}\",\"{item.CreatedAt:yyyy-MM-dd HH:mm:ss}\"");
        }

        return new ExportResult
        {
            Content = Encoding.UTF8.GetBytes(sb.ToString()),
            ContentType = "text/csv",
            FileName = $"ticket-{ticketNumber}-history.csv"
        };
    }

    private static ExportResult ExportToPdf(TicketExportData data, string ticketNumber)
    {
        // For MVP, generate a simple text-based PDF representation
        // In production, use a proper PDF library like QuestPDF or iTextSharp
        var sb = new StringBuilder();

        sb.AppendLine($"TICKET HISTORY EXPORT");
        sb.AppendLine($"=====================");
        sb.AppendLine();
        sb.AppendLine($"Ticket Number: {data.TicketNumber}");
        sb.AppendLine($"Title: {data.Title}");
        sb.AppendLine($"Status: {data.Status}");
        sb.AppendLine($"Priority: {data.Priority}");
        sb.AppendLine($"Created: {data.CreatedAt:yyyy-MM-dd HH:mm:ss}");

        if (data.ResolvedAt.HasValue)
            sb.AppendLine($"Resolved: {data.ResolvedAt:yyyy-MM-dd HH:mm:ss}");

        if (data.ClosedAt.HasValue)
            sb.AppendLine($"Closed: {data.ClosedAt:yyyy-MM-dd HH:mm:ss}");

        sb.AppendLine();
        sb.AppendLine("Description:");
        sb.AppendLine(data.Description);

        sb.AppendLine();
        sb.AppendLine("HISTORY");
        sb.AppendLine("-------");

        foreach (var item in data.History)
        {
            sb.AppendLine();
            sb.AppendLine($"[{item.CreatedAt:yyyy-MM-dd HH:mm:ss}] {item.Type} by {item.UserName}:");
            sb.AppendLine(item.Content);
        }

        // Return as text/plain for MVP - real PDF generation would require additional dependencies
        return new ExportResult
        {
            Content = Encoding.UTF8.GetBytes(sb.ToString()),
            ContentType = "text/plain",
            FileName = $"ticket-{ticketNumber}-history.txt"
        };
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Replace("\"", "\"\"").Replace("\n", " ").Replace("\r", "");
    }
}

/// <summary>
/// Internal DTO for export data
/// </summary>
internal sealed record TicketExportData
{
    public string TicketNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ResolvedAt { get; init; }
    public DateTimeOffset? ClosedAt { get; init; }
    public List<HistoryExportItem> History { get; init; } = [];
}

/// <summary>
/// Internal DTO for history export item
/// </summary>
internal sealed record HistoryExportItem
{
    public string Type { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
}
