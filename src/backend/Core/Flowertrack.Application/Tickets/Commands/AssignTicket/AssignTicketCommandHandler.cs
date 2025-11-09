using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.AssignTicket;

/// <summary>
/// Handler for AssignTicketCommand
/// </summary>
public sealed class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, Result<bool>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignTicketCommandHandler> _logger;

    public AssignTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignTicketCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Assigning ticket {TicketId} to user {AssignedToUserId} by {AssignedBy}",
                request.TicketId,
                request.AssignedToUserId,
                request.AssignedBy);

            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);

            if (ticket == null || ticket.IsDeleted)
            {
                _logger.LogWarning("Ticket {TicketId} not found", request.TicketId);
                return Result.Failure<bool>("Ticket not found");
            }

            try
            {
                ticket.AssignTo(request.AssignedToUserId, request.AssignedBy);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Invalid argument when assigning ticket {TicketId}",
                    request.TicketId);

                return Result.Failure<bool>(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully assigned ticket {TicketId} to user {AssignedToUserId}",
                request.TicketId,
                request.AssignedToUserId);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error assigning ticket {TicketId}",
                request.TicketId);

            return Result.Failure<bool>($"Failed to assign ticket: {ex.Message}");
        }
    }
}
