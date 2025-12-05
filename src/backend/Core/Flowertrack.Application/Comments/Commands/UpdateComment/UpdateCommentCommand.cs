using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Comments.Commands.UpdateComment;

public record UpdateCommentCommand : IRequest<Result>
{
    public Guid CommentId { get; init; }
    public string Content { get; init; } = default!;
}
