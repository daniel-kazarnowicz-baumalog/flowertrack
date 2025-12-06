using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Tickets.Commands.BulkAssignTickets;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.BulkArchiveTickets;

/// <summary>
/// Handler for BulkArchiveTicketsCommand
/// US-014: Masowe akcje na zgłoszeniach
/// </summary>
public sealed class BulkArchiveTicketsCommandHandler
    : IRequestHandler<BulkArchiveTicketsCommand, Result<BulkOperationResult>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BulkArchiveTicketsCommandHandler> _logger;

    public BulkArchiveTicketsCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ILogger<BulkArchiveTicketsCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BulkOperationResult>> Handle(
        BulkArchiveTicketsCommand request,
        CancellationToken cancellationToken)
    {
        var failures = new List<BulkOperationFailure>();
        var successCount = 0;

        _logger.LogInformation(
            "Processing bulk archive of {Count} tickets by {ArchivedBy}",
            request.TicketIds.Count,
            request.ArchivedBy);

        var archiveReason = string.IsNullOrWhiteSpace(request.Reason)
            ? "Bulk archive operation"
            : request.Reason;

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
                        Error = "Ticket has already been archived"
                    });
                    continue;
                }

                ticket.Delete(archiveReason, request.ArchivedBy);
                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to archive ticket {TicketId}",
                    ticketId);

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
            "Bulk archive completed: {SuccessCount}/{TotalCount} succeeded, {FailureCount} failed",
            result.SuccessCount,
            result.TotalCount,
            result.FailureCount);

        return Result.Success(result);
    }
}
