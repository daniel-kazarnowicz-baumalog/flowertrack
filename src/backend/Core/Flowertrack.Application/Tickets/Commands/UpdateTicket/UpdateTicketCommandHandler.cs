using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.UpdateTicket;

/// <summary>
/// Handler for UpdateTicketCommand
/// Updates ticket's basic information with validation and authorization
/// </summary>
public sealed class UpdateTicketCommandHandler
    : IRequestHandler<UpdateTicketCommand, Result<bool>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTicketCommandHandler> _logger;

    public UpdateTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTicketCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        UpdateTicketCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get ticket
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket {TicketId} not found", request.TicketId);
                return Result.Failure<bool>("Ticket not found");
            }

            _logger.LogInformation(
                "Updating ticket {TicketId} by user {UserId}",
                request.TicketId,
                request.UpdatedBy);

            // Update ticket using domain method
            ticket.Update(
                title: request.Title,
                description: request.Description,
                priority: request.Priority,
                userId: request.UpdatedBy);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully updated ticket {TicketId}",
                request.TicketId);

            return Result.Success(true);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error updating ticket {TicketId}", request.TicketId);
            return Result.Failure<bool>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ticket {TicketId}", request.TicketId);
            return Result.Failure<bool>($"Failed to update ticket: {ex.Message}");
        }
    }
}
