using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.BulkAssignTickets;

/// <summary>
/// Command to assign multiple tickets to a service technician
/// US-014: Masowe akcje na zgłoszeniach
/// </summary>
public sealed record BulkAssignTicketsCommand : IRequest<Result<BulkOperationResult>>
{
    /// <summary>
    /// List of ticket IDs to assign
    /// </summary>
    public IReadOnlyList<Guid> TicketIds { get; init; } = [];

    /// <summary>
    /// User ID of the service technician to assign tickets to
    /// </summary>
    public Guid AssignToUserId { get; init; }

    /// <summary>
    /// User ID performing the bulk assignment
    /// </summary>
    public Guid AssignedBy { get; init; }
}

/// <summary>
/// Result of a bulk operation
/// </summary>
public sealed record BulkOperationResult
{
    /// <summary>
    /// Total number of items processed
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Number of items successfully processed
    /// </summary>
    public int SuccessCount { get; init; }

    /// <summary>
    /// Number of items that failed to process
    /// </summary>
    public int FailureCount { get; init; }

    /// <summary>
    /// Details of failed items
    /// </summary>
    public IReadOnlyList<BulkOperationFailure> Failures { get; init; } = [];
}

/// <summary>
/// Details of a failed bulk operation item
/// </summary>
public sealed record BulkOperationFailure
{
    /// <summary>
    /// ID of the item that failed
    /// </summary>
    public Guid ItemId { get; init; }

    /// <summary>
    /// Error message describing why the item failed
    /// </summary>
    public string Error { get; init; } = string.Empty;
}
