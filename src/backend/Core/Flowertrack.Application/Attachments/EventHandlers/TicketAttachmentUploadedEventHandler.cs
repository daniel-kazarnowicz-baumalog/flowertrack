using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Domain.Entities.Tickets;
using Flowertrack.Domain.Events;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Attachments.EventHandlers;

public class TicketAttachmentUploadedEventHandler : INotificationHandler<TicketAttachmentUploadedEvent>
{
    private readonly ITicketHistoryRepository _historyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TicketAttachmentUploadedEventHandler> _logger;

    public TicketAttachmentUploadedEventHandler(
        ITicketHistoryRepository historyRepository,
        IUnitOfWork unitOfWork,
        ILogger<TicketAttachmentUploadedEventHandler> logger)
    {
        _historyRepository = historyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(TicketAttachmentUploadedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: Syncing attachment upload {AttachmentId} to Ticket History", notification.AttachmentId);
        /*
        var history = TicketHistory.CreateAttachment(
            notification.TicketId,
            notification.FileName,
            notification.StoragePath,
            notification.FileSizeBytes,
            notification.UploadedBy,
            "User " + notification.UploadedBy.ToString().Substring(0, 8),
            "Unknown"
        );
        
        await _historyRepository.AddAsync(history, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        */
        await Task.CompletedTask;
    }
}
