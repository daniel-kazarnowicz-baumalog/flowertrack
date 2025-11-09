namespace Flowertrack.Contracts.Tickets.Responses;

/// <summary>
/// Response for a single attachment
/// </summary>
public sealed record AttachmentResponse
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
    /// User name who uploaded the file
    /// </summary>
    public string UploadedByName { get; init; } = string.Empty;

    /// <summary>
    /// When the attachment was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Download URL
    /// </summary>
    public string DownloadUrl { get; init; } = string.Empty;
}
