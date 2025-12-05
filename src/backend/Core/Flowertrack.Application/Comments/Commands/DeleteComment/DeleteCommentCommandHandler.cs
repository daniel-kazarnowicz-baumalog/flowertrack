using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Comments.Commands.DeleteComment;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteCommentCommandHandler> _logger;

    public DeleteCommentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<DeleteCommentCommandHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId == null)
        {
            return Result.Failure("User ID not found or invalid");
        }
        
        var userId = _currentUserService.UserId.Value;

        var comment = await _context.TicketComments
            .FindAsync(new object[] { request.CommentId }, cancellationToken);

        if (comment == null)
        {
            return Result.Failure($"Comment with ID {request.CommentId} not found.");
        }

        try
        {
            comment.Delete(userId);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
             _logger.LogWarning(ex, "Invalid operation during comment deletion {CommentId}", request.CommentId);
             return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error deleting comment {CommentId}", request.CommentId);
             return Result.Failure("Failed to delete comment.");
        }

        return Result.Success();
    }
}
