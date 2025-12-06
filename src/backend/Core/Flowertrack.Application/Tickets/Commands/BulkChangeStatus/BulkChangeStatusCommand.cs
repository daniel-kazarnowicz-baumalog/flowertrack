using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Tickets.Commands.BulkAssignTickets;
using Flowertrack.Domain.Enums;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.BulkChangeStatus;

/// <summary>
/// Command to change status of multiple tickets
/// US-014: Masowe akcje na zgłoszeniach
/// </summary>
public sealed record BulkChangeStatusCommand : IRequest<Result<BulkOperationResult>>
{
    /// <summary>
    /// List of ticket IDs to update
    /// </summary>
    public IReadOnlyList<Guid> TicketIds { get; init; } = [];

    /// <summary>
    /// New status to apply to all tickets
    /// </summary>
    public TicketStatus NewStatus { get; init; }

    /// <summary>
    /// Reason for status change (required for Resolved/Closed)
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// User ID performing the bulk status change
    /// </summary>
    public Guid ChangedBy { get; init; }
}
