using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Comments.Commands.AddComment;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;
    private readonly ILogger<AddCommentCommandHandler> _logger;

    public AddCommentCommandHandler(
        IApplicationDbContext context,
        IUserContext userContext,
        ILogger<AddCommentCommandHandler> logger)
    {
        _context = context;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var userIdString = _userContext.UserId;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Result.Failure<Guid>("User ID not found or invalid");
        }

        // Check if ticket exists within RLS policies will be handled by the database save ideally, 
        // but for better error messages we might want to check existence.
        // However, RLS policies 'insert' might fail if user doesn't have access.
        // But here we are just creating an entity. The saving happens later.
        
        // According to our plan and patterns, we should rely on Domain Business Logic.
        // The TicketComment.Create factory method should be used.

        var commentResult = TicketComment.Create(
            request.TicketId,
            userId,
            request.Content,
            request.IsInternal
        );

        if (commentResult.IsFailure)
        {
            return Result.Failure<Guid>(commentResult.Error);
        }

        var comment = commentResult.Value;

        // Add to context
        _context.TicketComments.Add(comment);

        try 
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Error creating comment for ticket {TicketId}", request.TicketId);
             // With RLS, if the user shouldn't be adding this comment (e.g. Org User adding Internal comment), 
             // the DB save might throw or rows affected will be 0 ? 
             // RLS usually raises an error or silently ignores depending on config.
             // If silent ignore, SaveChangesAsync returns 0?
             // Assuming RLS policy raises exception or we let it bubble up as failure.
             return Result.Failure<Guid>("Failed to add comment. You may not have permission.");
        }

        return Result.Success(comment.Id);
    }
}
