using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Raised when a file attachment is uploaded to a ticket
/// </summary>
public sealed class TicketAttachmentUploadedEvent : DomainEvent
{
    public TicketAttachmentUploadedEvent(
        Guid attachmentId,
        Guid ticketId,
        Guid uploadedBy,
        string fileName,
        long fileSizeBytes,
        DateTimeOffset occurredAt) : base(ticketId)
    {
        AttachmentId = attachmentId;
        TicketId = ticketId;
        UploadedBy = uploadedBy;
        FileName = fileName;
        FileSizeBytes = fileSizeBytes;
    }

    public Guid AttachmentId { get; }
    public Guid TicketId { get; }
    public Guid UploadedBy { get; }
    public string FileName { get; }
    public long FileSizeBytes { get; }
}
