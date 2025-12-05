using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Comments.Commands.DeleteComment;

public record DeleteCommentCommand(Guid CommentId) : IRequest<Result>;
