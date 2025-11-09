namespace Flowertrack.Application.Tickets.Queries.GetTicketAttachments;

/// <summary>
/// DTO for ticket attachment
/// </summary>
public sealed record AttachmentDto
{
    /// <summary>
    /// Attachment ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Ticket ID
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>
    /// MIME content type
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; init; }

    /// <summary>
    /// User ID who uploaded the file
    /// </summary>
    public Guid UploadedBy { get; init; }

    /// <summary>
    /// User name who uploaded the file (denormalized for display)
    /// </summary>
    public string UploadedByName { get; init; } = string.Empty;

    /// <summary>
    /// When the attachment was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Storage path (for download)
    /// </summary>
    public string StoragePath { get; init; } = string.Empty;
}
