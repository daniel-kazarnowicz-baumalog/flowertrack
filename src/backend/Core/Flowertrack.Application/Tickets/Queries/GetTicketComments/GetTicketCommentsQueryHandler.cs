using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Queries.GetTicketComments;

/// <summary>
/// Handler for GetTicketCommentsQuery
/// Retrieves paginated comments for a ticket
/// </summary>
public sealed class GetTicketCommentsQueryHandler
    : IRequestHandler<GetTicketCommentsQuery, Result<PagedResult<CommentDto>>>
{
    private readonly ITicketCommentRepository _commentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly ILogger<GetTicketCommentsQueryHandler> _logger;

    public GetTicketCommentsQueryHandler(
        ITicketCommentRepository commentRepository,
        ITicketRepository ticketRepository,
        IServiceUserRepository serviceUserRepository,
        IOrganizationUserRepository organizationUserRepository,
        ILogger<GetTicketCommentsQueryHandler> logger)
    {
        _commentRepository = commentRepository;
        _ticketRepository = ticketRepository;
        _serviceUserRepository = serviceUserRepository;
        _organizationUserRepository = organizationUserRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResult<CommentDto>>> Handle(
        GetTicketCommentsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Getting comments for ticket {TicketId}, page {PageNumber}",
                request.TicketId,
                request.PageNumber);

            // Verify ticket exists
            var ticketExists = await _ticketRepository.ExistsAsync(request.TicketId, cancellationToken);
            if (!ticketExists)
            {
                _logger.LogWarning("Ticket {TicketId} not found", request.TicketId);
                return Result.Failure<PagedResult<CommentDto>>("Ticket not found");
            }

            // Validate page size
            var pageSize = Math.Min(request.PageSize, 200);

            // Get comments from repository
            var (comments, totalCount) = await _commentRepository.GetByTicketIdAsync(
                request.TicketId,
                request.IncludeInternal,
                request.PageNumber,
                pageSize,
                cancellationToken);

            // Map to DTOs with user names
            var commentDtos = new List<CommentDto>();
            foreach (var comment in comments)
            {
                var userName = await GetUserNameAsync(comment.UserId, cancellationToken);
                
                commentDtos.Add(new CommentDto
                {
                    Id = comment.Id,
                    TicketId = comment.TicketId,
                    UserId = comment.UserId,
                    UserName = userName,
                    Content = comment.Content,
                    IsInternal = comment.IsInternal,
                    CreatedAt = comment.CreatedAt,
                    UpdatedAt = comment.UpdatedAt
                });
            }

            var pagedResult = new PagedResult<CommentDto>(
                commentDtos,
                request.PageNumber,
                pageSize,
                totalCount);

            return Result.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments for ticket {TicketId}", request.TicketId);
            return Result.Failure<PagedResult<CommentDto>>($"Failed to get comments: {ex.Message}");
        }
    }

    private async Task<string> GetUserNameAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Try to find as service user first
        var serviceUser = await _serviceUserRepository.GetByIdAsync(userId, cancellationToken);
        if (serviceUser != null)
        {
            return $"{serviceUser.FirstName} {serviceUser.LastName}";
        }

        // Try to find as organization user
        var orgUser = await _organizationUserRepository.GetByIdAsync(userId, cancellationToken);
        if (orgUser != null)
        {
            return $"{orgUser.FirstName} {orgUser.LastName}";
        }

        return "Unknown User";
    }
}
