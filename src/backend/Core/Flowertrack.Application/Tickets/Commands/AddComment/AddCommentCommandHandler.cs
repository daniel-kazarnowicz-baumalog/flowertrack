using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities.Tickets;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.AddComment;

/// <summary>
/// Handler for AddCommentCommand
/// </summary>
public sealed class AddCommentCommandHandler(
    ITicketRepository ticketRepository,
    ITicketHistoryRepository historyRepository,
    IUnitOfWork unitOfWork,
    ILogger<AddCommentCommandHandler> logger)
    : IRequestHandler<AddCommentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        AddCommentCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding comment to ticket {TicketId} by user {UserId}", 
            request.TicketId, request.UserId);

        // Validate content
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result.Failure<Guid>("Comment content cannot be empty");
        }

        if (request.Content.Length > 4000)
        {
            return Result.Failure<Guid>("Comment content cannot exceed 4000 characters");
        }

        // Verify ticket exists
        var ticketExists = await ticketRepository.ExistsAsync(request.TicketId, cancellationToken);
        if (!ticketExists)
        {
            logger.LogWarning("Ticket {TicketId} not found", request.TicketId);
            return Result.Failure<Guid>("Ticket not found");
        }

        // Create comment history entry
        var comment = TicketHistory.CreateComment(
            request.TicketId,
            request.Content,
            request.UserId,
            request.UserName,
            request.UserType,
            request.IsInternal
        );

        await historyRepository.AddAsync(comment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Comment {CommentId} added to ticket {TicketId}", 
            comment.Id, request.TicketId);

        return Result.Success(comment.Id);
    }
}
