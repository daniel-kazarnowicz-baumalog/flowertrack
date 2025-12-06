using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Queries.ExportTicketHistory;

/// <summary>
/// Query to export ticket history in various formats
/// US-022: Eksport historii ticketu
/// </summary>
public sealed record ExportTicketHistoryQuery : IRequest<Result<ExportResult>>
{
    /// <summary>
    /// Ticket ID to export history for
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// Export format (pdf, csv, json)
    /// </summary>
    public ExportFormat Format { get; init; } = ExportFormat.Json;

    /// <summary>
    /// User ID making the request (for authorization)
    /// </summary>
    public Guid RequestedBy { get; init; }
}

/// <summary>
/// Supported export formats
/// </summary>
public enum ExportFormat
{
    /// <summary>
    /// JSON format
    /// </summary>
    Json,

    /// <summary>
    /// CSV format
    /// </summary>
    Csv,

    /// <summary>
    /// PDF format
    /// </summary>
    Pdf
}

/// <summary>
/// Result of an export operation
/// </summary>
public sealed record ExportResult
{
    /// <summary>
    /// File content as byte array
    /// </summary>
    public byte[] Content { get; init; } = [];

    /// <summary>
    /// MIME type of the content
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// Suggested filename for download
    /// </summary>
    public string FileName { get; init; } = string.Empty;
}
