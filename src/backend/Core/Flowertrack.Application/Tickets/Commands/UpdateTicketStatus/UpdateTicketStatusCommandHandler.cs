using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Common;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.UpdateTicketStatus;

/// <summary>
/// Handler for UpdateTicketStatusCommand
/// </summary>
public sealed class UpdateTicketStatusCommandHandler : IRequestHandler<UpdateTicketStatusCommand, Result<bool>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTicketStatusCommandHandler> _logger;

    public UpdateTicketStatusCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTicketStatusCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Updating status for ticket {TicketId} to {NewStatus}",
                request.TicketId,
                request.NewStatus);

            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);

            if (ticket == null || ticket.IsDeleted)
            {
                _logger.LogWarning("Ticket {TicketId} not found", request.TicketId);
                return Result.Failure<bool>("Ticket not found");
            }

            try
            {
                ticket.UpdateStatus(request.NewStatus, request.Reason, request.ChangedBy);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Invalid status transition for ticket {TicketId} from {OldStatus} to {NewStatus}",
                    request.TicketId,
                    ticket.Status,
                    request.NewStatus);

                return Result.Failure<bool>(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Invalid argument when updating status for ticket {TicketId}",
                    request.TicketId);

                return Result.Failure<bool>(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully updated status for ticket {TicketId} to {NewStatus}. Reason: {Reason}",
                request.TicketId,
                request.NewStatus,
                request.Reason ?? "(none)");

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating status for ticket {TicketId}",
                request.TicketId);

            return Result.Failure<bool>($"Failed to update ticket status: {ex.Message}");
        }
    }
}
