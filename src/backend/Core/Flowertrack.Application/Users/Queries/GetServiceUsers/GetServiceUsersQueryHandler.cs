using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Users.Queries.GetServiceUsers;

public class GetServiceUsersQueryHandler : IRequestHandler<GetServiceUsersQuery, Result<List<ServiceUserDto>>>
{
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly ITicketRepository _ticketRepository;

    public GetServiceUsersQueryHandler(
        IServiceUserRepository serviceUserRepository,
        ITicketRepository ticketRepository)
    {
        _serviceUserRepository = serviceUserRepository;
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<List<ServiceUserDto>>> Handle(GetServiceUsersQuery request, CancellationToken ct)
    {
        var users = await _serviceUserRepository.GetAllAsync(ct);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<UserStatus>(request.Status, true, out var status))
        {
            users = users.Where(u => u.Status == status).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            users = users.Where(u => 
                u.FirstName.ToLower().Contains(searchLower) ||
                u.LastName.ToLower().Contains(searchLower) ||
                u.Email.Value.ToLower().Contains(searchLower) ||
                (u.Specialization?.ToLower().Contains(searchLower) ?? false)
            ).ToList();
        }

        // Pagination
        var pagedUsers = users
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = new List<ServiceUserDto>();

        foreach (var user in pagedUsers)
        {
            // Get active tickets count
            var tickets = await _ticketRepository.GetByAssignedUserIdAsync(user.Id, ct);
            var activeTicketsCount = tickets.Count(t => 
                t.Status != TicketStatus.Closed && 
                t.Status != TicketStatus.Resolved);

            dtos.Add(new ServiceUserDto
            {
                Id = user.Id,
                Email = user.Email.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Specialization = user.Specialization,
                Status = user.Status.ToString(),
                IsAvailable = user.IsAvailable,
                ActiveTicketsCount = activeTicketsCount,
                LastActivity = user.UpdatedAt ?? user.CreatedAt,
                CreatedAt = user.CreatedAt
            });
        }

        return Result.Success(dtos);
    }
}
