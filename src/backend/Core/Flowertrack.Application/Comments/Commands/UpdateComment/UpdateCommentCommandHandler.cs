using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Comments.Commands.UpdateComment;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;
    private readonly ILogger<UpdateCommentCommandHandler> _logger;

    public UpdateCommentCommandHandler(
        IApplicationDbContext context,
        IUserContext userContext,
        ILogger<UpdateCommentCommandHandler> logger)
    {
        _context = context;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var userIdString = _userContext.UserId;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Result.Failure("User ID not found or invalid");
        }

        var comment = await _context.TicketComments
            .FindAsync(new object[] { request.CommentId }, cancellationToken);

        if (comment == null)
        {
            return Result.Failure($"Comment with ID {request.CommentId} not found.");
        }

        try
        {
            comment.Update(request.Content, userId);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation during comment update {CommentId}", request.CommentId);
            return Result.Failure(ex.Message);
        }
        catch (ArgumentException ex)
        {
             _logger.LogWarning(ex, "Validation error during comment update {CommentId}", request.CommentId);
             return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error updating comment {CommentId}", request.CommentId);
             return Result.Failure("Failed to update comment.");
        }

        return Result.Success();
    }
}
