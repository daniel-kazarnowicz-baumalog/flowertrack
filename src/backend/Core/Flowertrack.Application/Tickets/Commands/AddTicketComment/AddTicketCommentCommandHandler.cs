using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.AddTicketComment;

/// <summary>
/// Handler for AddTicketCommentCommand
/// Creates a new comment on a ticket using TicketComment entity
/// </summary>
public sealed class AddTicketCommentCommandHandler
    : IRequestHandler<AddTicketCommentCommand, Result<Guid>>
{
    private readonly ITicketCommentRepository _commentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddTicketCommentCommandHandler> _logger;

    public AddTicketCommentCommandHandler(
        ITicketCommentRepository commentRepository,
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddTicketCommentCommandHandler> logger)
    {
        _commentRepository = commentRepository;
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        AddTicketCommentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Adding comment to ticket {TicketId} by user {UserId}",
                request.TicketId,
                request.UserId);

            // Verify ticket exists
            var ticketExists = await _ticketRepository.ExistsAsync(request.TicketId, cancellationToken);
            if (!ticketExists)
            {
                _logger.LogWarning("Ticket {TicketId} not found", request.TicketId);
                return Result.Failure<Guid>("Ticket not found");
            }

            // Create comment using domain factory method
            var comment = TicketComment.Create(
                ticketId: request.TicketId,
                userId: request.UserId,
                content: request.Content,
                isInternal: request.IsInternal);

            // Add comment to repository
            await _commentRepository.AddAsync(comment, cancellationToken);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully added comment {CommentId} to ticket {TicketId}",
                comment.Id,
                request.TicketId);

            return Result.Success(comment.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to ticket {TicketId}", request.TicketId);
            return Result.Failure<Guid>($"Failed to add comment: {ex.Message}");
        }
    }
}
