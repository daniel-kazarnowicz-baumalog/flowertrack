using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.DeleteTicket;

/// <summary>
/// Handler for DeleteTicketCommand
/// Performs soft delete by setting IsDeleted flag and audit information
/// </summary>
public sealed class DeleteTicketCommandHandler
    : IRequestHandler<DeleteTicketCommand, Result<bool>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteTicketCommandHandler> _logger;

    public DeleteTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteTicketCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        DeleteTicketCommand request,
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
                "Deleting ticket {TicketId} ({TicketNumber}) by user {UserId}. Reason: {Reason}",
                request.TicketId,
                ticket.TicketNumber.Value,
                request.DeletedBy,
                request.Reason ?? "No reason provided");

            // Perform soft delete using domain method
            ticket.Delete(request.Reason, request.DeletedBy);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully deleted ticket {TicketId} ({TicketNumber})",
                request.TicketId,
                ticket.TicketNumber.Value);

            return Result.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete ticket {TicketId}", request.TicketId);
            return Result.Failure<bool>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting ticket {TicketId}", request.TicketId);
            return Result.Failure<bool>($"Failed to delete ticket: {ex.Message}");
        }
    }
}
