using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Tickets.Commands.BulkAssignTickets;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.BulkChangeStatus;

/// <summary>
/// Handler for BulkChangeStatusCommand
/// US-014: Masowe akcje na zgłoszeniach
/// </summary>
public sealed class BulkChangeStatusCommandHandler
    : IRequestHandler<BulkChangeStatusCommand, Result<BulkOperationResult>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BulkChangeStatusCommandHandler> _logger;

    public BulkChangeStatusCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ILogger<BulkChangeStatusCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BulkOperationResult>> Handle(
        BulkChangeStatusCommand request,
        CancellationToken cancellationToken)
    {
        var failures = new List<BulkOperationFailure>();
        var successCount = 0;

        _logger.LogInformation(
            "Processing bulk status change of {Count} tickets to {NewStatus} by {ChangedBy}",
            request.TicketIds.Count,
            request.NewStatus,
            request.ChangedBy);

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

                // Validate state transition
                if (!ticket.IsValidStatusTransition(request.NewStatus, DateTimeOffset.UtcNow))
                {
                    failures.Add(new BulkOperationFailure
                    {
                        ItemId = ticketId,
                        Error = $"Cannot transition from {ticket.Status} to {request.NewStatus}"
                    });
                    continue;
                }

                ticket.UpdateStatus(request.NewStatus, request.Reason, request.ChangedBy);
                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to change status of ticket {TicketId} to {NewStatus}",
                    ticketId,
                    request.NewStatus);

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
            "Bulk status change completed: {SuccessCount}/{TotalCount} succeeded, {FailureCount} failed",
            result.SuccessCount,
            result.TotalCount,
            result.FailureCount);

        return Result.Success(result);
    }
}
