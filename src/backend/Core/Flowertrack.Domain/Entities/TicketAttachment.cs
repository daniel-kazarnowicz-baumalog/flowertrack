using Flowertrack.Domain.Common;
using Flowertrack.Domain.Events;

namespace Flowertrack.Domain.Entities;

/// <summary>
/// Represents a file attachment on a service ticket
/// </summary>
public sealed class TicketAttachment : AuditableEntity<Guid>
{
    private TicketAttachment() : base(Guid.NewGuid()) { } // EF Core constructor

    private TicketAttachment(
        Guid id,
        Guid ticketId,
        Guid uploadedBy,
        string fileName,
        string storagePath,
        string contentType,
        long fileSize) : base(id)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be empty", nameof(fileName));
        }

        if (fileName.Length > 255)
        {
            throw new ArgumentException("File name cannot exceed 255 characters", nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(storagePath))
        {
            throw new ArgumentException("Storage path cannot be empty", nameof(storagePath));
        }

        if (fileSize <= 0)
        {
            throw new ArgumentException("File size must be greater than zero", nameof(fileSize));
        }

        if (fileSize > 52428800) // 50 MB limit
        {
            throw new ArgumentException("File size cannot exceed 50 MB", nameof(fileSize));
        }

        Id = id;
        TicketId = ticketId;
        UploadedBy = uploadedBy;
        FileName = fileName;
        StoragePath = storagePath;
        ContentType = contentType ?? "application/octet-stream";
        FileSizeBytes = fileSize;
        SetCreatedAudit(uploadedBy);
    }

    /// <summary>
    /// ID of the ticket this attachment belongs to
    /// </summary>
    public Guid TicketId { get; private set; }

    /// <summary>
    /// ID of the user who uploaded the attachment
    /// </summary>
    public Guid UploadedBy { get; private set; }

    /// <summary>
    /// Original file name (max 255 characters)
    /// </summary>
    public string FileName { get; private set; } = string.Empty;

    /// <summary>
    /// Path/URL to file in Supabase Storage
    /// </summary>
    public string StoragePath { get; private set; } = string.Empty;

    /// <summary>
    /// MIME type of the file (e.g., image/png, application/pdf)
    /// </summary>
    public string ContentType { get; private set; } = string.Empty;

    /// <summary>
    /// File size in bytes (max 50 MB = 52428800 bytes)
    /// </summary>
    public long FileSizeBytes { get; private set; }

    /// <summary>
    /// Navigation property to ticket
    /// </summary>
    public Ticket Ticket { get; private set; } = null!;

    /// <summary>
    /// Factory method to create a new attachment
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="uploadedBy">User ID who is uploading the file</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="storagePath">Path/URL in Supabase Storage</param>
    /// <param name="contentType">MIME type</param>
    /// <param name="fileSize">File size in bytes</param>
    /// <returns>New TicketAttachment instance</returns>
    public static TicketAttachment Create(
        Guid ticketId,
        Guid uploadedBy,
        string fileName,
        string storagePath,
        string contentType,
        long fileSize)
    {
        var attachment = new TicketAttachment(
            Guid.NewGuid(),
            ticketId,
            uploadedBy,
            fileName,
            storagePath,
            contentType,
            fileSize);

        attachment.RaiseDomainEvent(new TicketAttachmentUploadedEvent(
            attachment.Id,
            attachment.TicketId,
            attachment.UploadedBy,
            attachment.FileName,
            attachment.FileSizeBytes,
            DateTimeOffset.UtcNow));

        return attachment;
    }

    /// <summary>
    /// Soft deletes the attachment
    /// Storage cleanup should be handled by application layer
    /// </summary>
    /// <param name="userId">User performing the deletion</param>
    public void Delete(Guid userId)
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("Attachment is already deleted");
        }

        SetDeletedAudit(userId);

        RaiseDomainEvent(new TicketAttachmentDeletedEvent(
            Id,
            TicketId,
            userId,
            StoragePath,
            DateTimeOffset.UtcNow));
    }
}
