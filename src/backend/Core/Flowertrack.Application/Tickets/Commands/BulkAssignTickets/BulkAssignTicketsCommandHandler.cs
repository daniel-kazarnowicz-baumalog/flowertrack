using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.BulkAssignTickets;

/// <summary>
/// Handler for BulkAssignTicketsCommand
/// US-014: Masowe akcje na zgłoszeniach
/// </summary>
public sealed class BulkAssignTicketsCommandHandler
    : IRequestHandler<BulkAssignTicketsCommand, Result<BulkOperationResult>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BulkAssignTicketsCommandHandler> _logger;

    public BulkAssignTicketsCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ILogger<BulkAssignTicketsCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BulkOperationResult>> Handle(
        BulkAssignTicketsCommand request,
        CancellationToken cancellationToken)
    {
        var failures = new List<BulkOperationFailure>();
        var successCount = 0;

        _logger.LogInformation(
            "Processing bulk assignment of {Count} tickets to user {AssignToUserId} by {AssignedBy}",
            request.TicketIds.Count,
            request.AssignToUserId,
            request.AssignedBy);

        foreach (var ticketId in request.TicketIds)
        {
            try
            {
                var ticket = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);
                if (ticket == null)
                {
                    failures.Add(new BulkOperationFailure
                    {
                        ItemId = ticketId,
                        Error = "Ticket not found"
                    });
                    continue;
                }

                if (ticket.IsDeleted)
                {
                    failures.Add(new BulkOperationFailure
                    {
                        ItemId = ticketId,
                        Error = "Ticket has been deleted"
                    });
                    continue;
                }

                ticket.AssignTo(request.AssignToUserId, request.AssignedBy);
                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to assign ticket {TicketId} to user {AssignToUserId}",
                    ticketId,
                    request.AssignToUserId);

                failures.Add(new BulkOperationFailure
                {
                    ItemId = ticketId,
                    Error = ex.Message
                });
            }
        }

        // Save all changes at once
        if (successCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var result = new BulkOperationResult
        {
            TotalCount = request.TicketIds.Count,
            SuccessCount = successCount,
            FailureCount = failures.Count,
            Failures = failures
        };

        _logger.LogInformation(
            "Bulk assignment completed: {SuccessCount}/{TotalCount} succeeded, {FailureCount} failed",
            result.SuccessCount,
            result.TotalCount,
            result.FailureCount);

        return Result.Success(result);
    }
}
