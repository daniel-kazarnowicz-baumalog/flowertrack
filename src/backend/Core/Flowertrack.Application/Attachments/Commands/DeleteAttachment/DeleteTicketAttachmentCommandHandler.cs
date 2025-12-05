using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Attachments.Commands.DeleteAttachment;

/// <summary>
/// Handler for DeleteTicketAttachmentCommand
/// Soft deletes an attachment from a ticket
/// Note: Physical file is kept in storage for audit purposes
/// </summary>
public sealed class DeleteTicketAttachmentCommandHandler
    : IRequestHandler<DeleteTicketAttachmentCommand, Result>
{
    private readonly ITicketAttachmentRepository _attachmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteTicketAttachmentCommandHandler> _logger;

    public DeleteTicketAttachmentCommandHandler(
        ITicketAttachmentRepository attachmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteTicketAttachmentCommandHandler> logger)
    {
        _attachmentRepository = attachmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        DeleteTicketAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Deleting attachment {AttachmentId} by user {UserId}",
                request.AttachmentId,
                request.UserId);

            // Get the attachment
            var attachment = await _attachmentRepository.GetByIdAsync(request.AttachmentId, cancellationToken);
            if (attachment == null)
            {
                _logger.LogWarning("Attachment {AttachmentId} not found", request.AttachmentId);
                return Result.Failure("Attachment not found");
            }

            // Delete the attachment (soft delete)
            // Physical file remains in storage for audit purposes
            attachment.Delete(request.UserId);

            // Update in repository
            await _attachmentRepository.UpdateAsync(attachment, cancellationToken);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully deleted attachment {AttachmentId}",
                attachment.Id);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation when deleting attachment {AttachmentId}", request.AttachmentId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting attachment {AttachmentId}", request.AttachmentId);
            return Result.Failure($"Failed to delete attachment: {ex.Message}");
        }
    }
}
