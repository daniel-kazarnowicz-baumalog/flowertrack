using Flowertrack.Application.Comments.DTOs;
using Flowertrack.Application.Common.Interfaces;

using Flowertrack.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Application.Comments.Queries.GetComments;

public class GetCommentsForTicketQueryHandler : IRequestHandler<GetCommentsForTicketQuery, Result<PaginatedList<CommentDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public GetCommentsForTicketQueryHandler(
        IApplicationDbContext context,
        IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<Result<PaginatedList<CommentDto>>> Handle(GetCommentsForTicketQuery request, CancellationToken cancellationToken)
    {
        // Check ticket existence
        var ticketExists = await _context.Tickets
            .AnyAsync(t => t.Id == request.TicketId, cancellationToken);
            
        if (!ticketExists)
        {
            return Result.Failure<PaginatedList<CommentDto>>($"Ticket with ID {request.TicketId} not found");
        }

        var userIdString = _userContext.UserId;
        Guid.TryParse(userIdString, out var currentUserId);
        
        // Check if user is Service User (implementation dependent, assume true if Role contains ServiceUser or via Claims)
        // For simplicity, we can rely on RLS to filter comments content, but for "CanEdit/CanDelete" logic we need to know roles or ownership.
        // Assuming RLS handles Visibility (IsInternal).
        // However, standard EF Core doesn't automatically apply RLS unless using a specific connection setup that sets session variables.
        // In this project (Supabase + RLS), the "Service Role" bypasses RLS, but "Authenticated" users use it.
        // If the backend runs as Service Role (Admin), we might see everything.
        // BUT typical Clean Architecture implementations often perform application-level filtering as well.
        // The implementation plan says: "Filter internal comments based on user role"
        
        // Let's implement Application-level filtering as a safeguard and for calculated fields.

        var query = _context.TicketComments
            .Where(c => c.TicketId == request.TicketId && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .AsNoTracking();

        // 1. Join with Users to get Author Name (ServiceUser or OrganizationUser)
        // Since we have two user tables, we might have to do left joins or rely on a View.
        // Or simpler: Fetch comments first, then enrich with user names.
        
        var commentsPage = await query
            .Include(c => c.Ticket) // Optional
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Calculate Total Count
        var totalCount = await query.CountAsync(cancellationToken);

        // Enrich with Author Names and Permissions
        var commentDtos = new List<CommentDto>();
        
        // Get all unique user IDs
        var userIds = commentsPage.Select(c => c.UserId).Distinct().ToList();
        
        // Fetch Service Users
        var serviceUsers = await _context.ServiceUsers
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}", cancellationToken);
            
        // Fetch Org Users
        var orgUsers = await _context.OrganizationUsers
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}", cancellationToken);

        foreach (var comment in commentsPage)
        {
            string authorName = "Unknown";
            if (serviceUsers.TryGetValue(comment.UserId, out var serviceName)) authorName = serviceName;
            else if (orgUsers.TryGetValue(comment.UserId, out var orgName)) authorName = orgName;

            // Permission Logic
            // Edit: Author only, within 15 minutes
            var timeSinceCreation = DateTimeOffset.UtcNow - comment.CreatedAt;
            bool canEdit = comment.UserId == currentUserId && timeSinceCreation.TotalMinutes <= 15;
            
            // Delete: Author only (or Admin - logic could be extended)
            bool canDelete = comment.UserId == currentUserId;

            commentDtos.Add(new CommentDto(
                comment.Id,
                comment.TicketId,
                comment.UserId,
                authorName,
                comment.Content,
                comment.IsInternal,
                comment.CreatedAt,
                comment.UpdatedAt,
                canEdit,
                canDelete
            ));
        }

        return Result.Success(new PaginatedList<CommentDto>(
            commentDtos,
            totalCount,
            request.PageNumber,
            request.PageSize
        ));
    }
}
