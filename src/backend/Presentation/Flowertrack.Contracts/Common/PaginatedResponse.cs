namespace Flowertrack.Contracts.Common;

/// <summary>
/// Paginated response wrapper
/// </summary>
public sealed record PaginatedResponse<T>
{
    public List<T> Items { get; init; } = new();
    public PaginationMetadata Pagination { get; init; } = new();
}

/// <summary>
/// Pagination metadata
/// </summary>
public sealed record PaginationMetadata
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }
}
