using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.UpdateTicketComment;

/// <summary>
/// Handler for UpdateTicketCommentCommand
/// Updates an existing comment on a ticket
/// </summary>
public sealed class UpdateTicketCommentCommandHandler
    : IRequestHandler<UpdateTicketCommentCommand, Result<Guid>>
{
    private readonly ITicketCommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTicketCommentCommandHandler> _logger;

    public UpdateTicketCommentCommandHandler(
        ITicketCommentRepository commentRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTicketCommentCommandHandler> logger)
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        UpdateTicketCommentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Updating comment {CommentId} by user {UserId}",
                request.CommentId,
                request.UserId);

            // Get the comment
            var comment = await _commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
            if (comment == null)
            {
                _logger.LogWarning("Comment {CommentId} not found", request.CommentId);
                return Result.Failure<Guid>("Comment not found");
            }

            // Update the comment (domain method enforces author check)
            comment.Update(request.Content, request.UserId);

            // Update in repository
            await _commentRepository.UpdateAsync(comment, cancellationToken);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully updated comment {CommentId}",
                comment.Id);

            return Result.Success(comment.Id);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation when updating comment {CommentId}", request.CommentId);
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment {CommentId}", request.CommentId);
            return Result.Failure<Guid>($"Failed to update comment: {ex.Message}");
        }
    }
}
