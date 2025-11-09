using Flowertrack.Contracts.Common;

namespace Flowertrack.Contracts.Tickets.Responses;

/// <summary>
/// Response for getting ticket comments with pagination
/// </summary>
public sealed record GetCommentsResponse
{
    /// <summary>
    /// List of comments
    /// </summary>
    public List<CommentResponse> Comments { get; init; } = new();

    /// <summary>
    /// Pagination information
    /// </summary>
    public PaginationMetadata Pagination { get; init; } = new();
}
