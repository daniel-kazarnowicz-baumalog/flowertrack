using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Domain.Entities.Tickets;
using Flowertrack.Domain.Events;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Comments.EventHandlers;

public class TicketCommentAddedEventHandler : INotificationHandler<TicketCommentAddedEvent>
{
    private readonly ITicketHistoryRepository _historyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TicketCommentAddedEventHandler> _logger;

    public TicketCommentAddedEventHandler(
        ITicketHistoryRepository historyRepository,
        IUnitOfWork unitOfWork,
        ILogger<TicketCommentAddedEventHandler> logger)
    {
        _historyRepository = historyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(TicketCommentAddedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: Syncing new comment {CommentId} to Ticket History", notification.CommentId);
        /*
        var history = TicketHistory.CreateComment(
            notification.TicketId,
            notification.Content,
            notification.AuthorId,
            "User " + notification.AuthorId.ToString().Substring(0, 8), // Placeholder
            "Unknown", // Placeholder
            notification.IsInternal
        );

        await _historyRepository.AddAsync(history, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        */
        await Task.CompletedTask;
    }
}
