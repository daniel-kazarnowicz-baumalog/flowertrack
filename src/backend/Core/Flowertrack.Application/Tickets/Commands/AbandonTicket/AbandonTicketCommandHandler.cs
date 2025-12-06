using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.AbandonTicket;

/// <summary>
/// Handler for AbandonTicketCommand
/// US-043: Performs soft delete of ticket (marks as abandoned/deleted)
/// </summary>
public sealed class AbandonTicketCommandHandler
    : IRequestHandler<AbandonTicketCommand, Result<bool>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AbandonTicketCommandHandler> _logger;

    public AbandonTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ILogger<AbandonTicketCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        AbandonTicketCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket {TicketId} not found for abandonment", request.TicketId);
                return Result.Failure<bool>("Ticket not found");
            }

            _logger.LogInformation(
                "Abandoning ticket {TicketId} ({TicketNumber}) by user {UserId}. Reason: {Reason}",
                request.TicketId,
                ticket.TicketNumber.Value,
                request.AbandonedBy,
                request.Reason ?? "No reason provided");

            // Use soft delete with abandon reason
            var abandonReason = string.IsNullOrWhiteSpace(request.Reason)
                ? "Ticket abandoned by user"
                : $"Ticket abandoned: {request.Reason}";

            ticket.Delete(abandonReason, request.AbandonedBy);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully abandoned ticket {TicketId} ({TicketNumber})",
                request.TicketId,
                ticket.TicketNumber.Value);

            return Result.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot abandon ticket {TicketId}", request.TicketId);
            return Result.Failure<bool>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error abandoning ticket {TicketId}", request.TicketId);
            return Result.Failure<bool>($"Failed to abandon ticket: {ex.Message}");
        }
    }
}
