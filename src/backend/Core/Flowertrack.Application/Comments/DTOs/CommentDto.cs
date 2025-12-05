namespace Flowertrack.Application.Comments.DTOs;

public record CommentDto(
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
