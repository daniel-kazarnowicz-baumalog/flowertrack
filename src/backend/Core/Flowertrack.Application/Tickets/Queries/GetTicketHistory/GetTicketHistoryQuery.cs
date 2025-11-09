using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Queries.GetTicketHistory;

/// <summary>
/// Query to get ticket history entries
/// </summary>
public sealed record GetTicketHistoryQuery(
    Guid TicketId,
    bool IncludeInternal = false,
    int? PageNumber = null,
    int? PageSize = null
) : IRequest<Result<TicketHistoryResponse>>;

/// <summary>
/// Response DTO for ticket history
/// </summary>
public sealed record TicketHistoryResponse(
    IReadOnlyList<TicketHistoryItemDto> Items,
    int TotalCount
);

/// <summary>
/// DTO for a single history item
/// </summary>
public sealed record TicketHistoryItemDto(
    Guid Id,
    string HistoryType,
    Guid? UserId,
    string UserName,
    string? UserType,
    string? OldValue,
    string? NewValue,
    string? Content,
    string? AttachmentFileName,
    string? AttachmentFilePath,
    long? AttachmentFileSize,
    bool IsInternal,
    DateTimeOffset CreatedAt
);
