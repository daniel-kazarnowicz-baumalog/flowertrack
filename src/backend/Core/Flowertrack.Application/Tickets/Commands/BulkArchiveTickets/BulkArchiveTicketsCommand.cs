using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Tickets.Commands.BulkAssignTickets;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.BulkArchiveTickets;

/// <summary>
/// Command to archive (soft delete) multiple tickets
/// US-014: Masowe akcje na zgłoszeniach
/// </summary>
public sealed record BulkArchiveTicketsCommand : IRequest<Result<BulkOperationResult>>
{
    /// <summary>
    /// List of ticket IDs to archive
    /// </summary>
    public IReadOnlyList<Guid> TicketIds { get; init; } = [];

    /// <summary>
    /// Reason for archiving
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// User ID performing the bulk archive
    /// </summary>
    public Guid ArchivedBy { get; init; }
}
