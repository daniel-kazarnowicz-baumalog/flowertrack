using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.DeleteTicketComment;

/// <summary>
/// Handler for DeleteTicketCommentCommand
/// Soft deletes a comment from a ticket
/// </summary>
public sealed class DeleteTicketCommentCommandHandler
    : IRequestHandler<DeleteTicketCommentCommand, Result>
{
    private readonly ITicketCommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteTicketCommentCommandHandler> _logger;

    public DeleteTicketCommentCommandHandler(
        ITicketCommentRepository commentRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteTicketCommentCommandHandler> logger)
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        DeleteTicketCommentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Deleting comment {CommentId} by user {UserId}",
                request.CommentId,
                request.UserId);

            // Get the comment
            var comment = await _commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found", request.CommentId);
                return Result.Failure("Comment not found");
            }

            // Delete the comment (soft delete)
            comment.Delete(request.UserId);

            // Update in repository
            await _commentRepository.UpdateAsync(comment, cancellationToken);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully deleted comment {CommentId}",
                comment.Id);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation when deleting comment {CommentId}", request.CommentId);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId}", request.CommentId);
            return Result.Failure($"Failed to delete comment: {ex.Message}");
        }
    }
}
