using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Attachments.Commands.DeleteAttachment;

/// <summary>
/// Command to delete an attachment from a ticket (soft delete)
/// </summary>
public sealed record DeleteTicketAttachmentCommand : IRequest<Result>
{
    /// <summary>
    /// Attachment ID to delete
    /// </summary>
    public Guid AttachmentId { get; init; }

    /// <summary>
    /// User ID who is deleting the attachment
    /// </summary>
    public Guid UserId { get; init; }
}
