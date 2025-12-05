using Flowertrack.Application.Comments.DTOs;
using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Comments.Queries.GetComments;

public record GetCommentsForTicketQuery : IRequest<Result<PaginatedList<CommentDto>>>
{
    public Guid TicketId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
