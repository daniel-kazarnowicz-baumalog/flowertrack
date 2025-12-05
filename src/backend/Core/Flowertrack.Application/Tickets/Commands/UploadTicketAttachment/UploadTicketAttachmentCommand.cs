using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.UploadTicketAttachment;

/// <summary>
/// Command to upload an attachment to a ticket
/// </summary>
public sealed record UploadTicketAttachmentCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Ticket ID to upload attachment to
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// User ID who is uploading the file
    /// </summary>
    public Guid UploadedBy { get; init; }

    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>
    /// File content stream
    /// </summary>
    public Stream FileStream { get; init; } = Stream.Null;

    /// <summary>
    /// MIME content type
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; init; }
}
