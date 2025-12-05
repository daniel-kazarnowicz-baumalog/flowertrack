using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities.Tickets;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.AddNote;

/// <summary>
/// Handler for AddNoteCommand
/// </summary>
public sealed class AddNoteCommandHandler(
    ITicketRepository ticketRepository,
    ITicketHistoryRepository historyRepository,
    IUnitOfWork unitOfWork,
    ILogger<AddNoteCommandHandler> logger)
    : IRequestHandler<AddNoteCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        AddNoteCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding internal note to ticket {TicketId} by user {UserId}", 
            request.TicketId, request.UserId);

        // Validate content
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result.Failure<Guid>("Note content cannot be empty");
        }

        if (request.Content.Length > 4000)
        {
            return Result.Failure<Guid>("Note content cannot exceed 4000 characters");
        }

        // Verify ticket exists
        var ticketExists = await ticketRepository.ExistsAsync(request.TicketId, cancellationToken);
        if (!ticketExists)
        {
            logger.LogWarning("Ticket {TicketId} not found", request.TicketId);
            return Result.Failure<Guid>("Ticket not found");
        }

        // Only service users can add internal notes
        if (request.UserType != "ServiceUser")
        {
            logger.LogWarning("User {UserId} of type {UserType} attempted to add internal note", 
                request.UserId, request.UserType);
            return Result.Failure<Guid>("Only service users can add internal notes");
        }

        // Create note history entry
        var note = TicketHistory.CreateNote(
            request.TicketId,
            request.Content,
            request.UserId,
            request.UserName,
            request.UserType
        );

        await historyRepository.AddAsync(note, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Internal note {NoteId} added to ticket {TicketId}", 
            note.Id, request.TicketId);

        return Result.Success(note.Id);
    }
}
