using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Raised when a file attachment is deleted (soft delete with storage cleanup pending)
/// </summary>
public sealed class TicketAttachmentDeletedEvent : DomainEvent
{
    public TicketAttachmentDeletedEvent(
        Guid attachmentId,
        Guid ticketId,
        Guid deletedBy,
        string storagePath,
        DateTimeOffset occurredAt) : base(ticketId)
    {
        AttachmentId = attachmentId;
        TicketId = ticketId;
        DeletedBy = deletedBy;
        StoragePath = storagePath;
    }

    public Guid AttachmentId { get; }
    public Guid TicketId { get; }
    public Guid DeletedBy { get; }
    public string StoragePath { get; }
}
