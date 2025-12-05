using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Comments.Commands.AddComment;

public record AddCommentCommand : IRequest<Result<Guid>>
{
    public Guid TicketId { get; init; }
    public string Content { get; init; } = default!;
    public bool IsInternal { get; init; }
}
