namespace Flowertrack.Contracts.Tickets.Responses;

/// <summary>
/// Response for bulk operations on tickets
/// US-014: Masowe akcje na zgłoszeniach
/// </summary>
public sealed record BulkOperationResponse
{
    /// <summary>
    /// Total number of items processed
    /// </summary>
    /// <example>10</example>
    public int TotalCount { get; init; }

    /// <summary>
    /// Number of items successfully processed
    /// </summary>
    /// <example>8</example>
    public int SuccessCount { get; init; }

    /// <summary>
    /// Number of items that failed to process
    /// </summary>
    /// <example>2</example>
    public int FailureCount { get; init; }

    /// <summary>
    /// Details of failed items
    /// </summary>
    public IReadOnlyList<BulkOperationFailureDto> Failures { get; init; } = [];
}

/// <summary>
/// Details of a failed bulk operation item
/// </summary>
public sealed record BulkOperationFailureDto
{
    /// <summary>
    /// ID of the item that failed
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ItemId { get; init; }

    /// <summary>
    /// Error message describing why the item failed
    /// </summary>
    /// <example>Ticket not found</example>
    public string Error { get; init; } = string.Empty;
}
