using Flowertrack.Contracts.Common;

namespace Flowertrack.Contracts.Comments;

public record CommentResponse(
    Guid Id,
    Guid TicketId,
    Guid UserId,
    string AuthorName,
    string Content,
    bool IsInternal,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    bool CanEdit,
    bool CanDelete
);

/// <summary>
/// Response containing paginated list of comments
/// </summary>
public sealed record GetCommentsResponse
{
    public IReadOnlyList<CommentResponse> Items { get; init; } = Array.Empty<CommentResponse>();
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }
}
