using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Queries.GetTicketHistory;

/// <summary>
/// Handler for GetTicketHistoryQuery
/// </summary>
public sealed class GetTicketHistoryQueryHandler(
    ITicketHistoryRepository historyRepository,
    ITicketRepository ticketRepository,
    ILogger<GetTicketHistoryQueryHandler> logger)
    : IRequestHandler<GetTicketHistoryQuery, Result<TicketHistoryResponse>>
{
    public async Task<Result<TicketHistoryResponse>> Handle(
        GetTicketHistoryQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting history for ticket {TicketId}", request.TicketId);

        // Verify ticket exists
        var ticketExists = await ticketRepository.ExistsAsync(request.TicketId, cancellationToken);
        if (!ticketExists)
        {
            logger.LogWarning("Ticket {TicketId} not found", request.TicketId);
            return Result.Failure<TicketHistoryResponse>("Ticket not found");
        }

        // Get history - paginated or all
        IReadOnlyList<Domain.Entities.Tickets.TicketHistory> historyItems;
        int totalCount;

        if (request.PageNumber.HasValue && request.PageSize.HasValue)
        {
            var result = await historyRepository.GetPagedByTicketIdAsync(
                request.TicketId,
                request.PageNumber.Value,
                request.PageSize.Value,
                request.IncludeInternal,
                cancellationToken);

            historyItems = result.Items;
            totalCount = result.TotalCount;
        }
        else
        {
            historyItems = await historyRepository.GetByTicketIdAsync(
                request.TicketId,
                request.IncludeInternal,
                cancellationToken);

            totalCount = historyItems.Count;
        }

        // Map to DTOs
        var dtos = historyItems.Select(h => new TicketHistoryItemDto(
            h.Id,
            h.HistoryType.ToString(),
            h.UserId,
            h.UserName,
            h.UserType,
            h.OldValue,
            h.NewValue,
            h.Content,
            h.AttachmentFileName,
            h.AttachmentFilePath,
            h.AttachmentFileSize,
            h.IsInternal,
            h.CreatedAt
        )).ToList();

        logger.LogInformation("Retrieved {Count} history items for ticket {TicketId}", dtos.Count, request.TicketId);

        return Result.Success(new TicketHistoryResponse(dtos, totalCount));
    }
}
