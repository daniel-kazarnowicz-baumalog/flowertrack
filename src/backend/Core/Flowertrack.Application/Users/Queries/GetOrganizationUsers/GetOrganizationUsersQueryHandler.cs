using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Users.Queries.GetOrganizationUsers;

public class GetOrganizationUsersQueryHandler : IRequestHandler<GetOrganizationUsersQuery, Result<List<OrganizationUserDto>>>
{
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly ITicketRepository _ticketRepository;

    public GetOrganizationUsersQueryHandler(
        IOrganizationUserRepository organizationUserRepository,
        ITicketRepository ticketRepository)
    {
        _organizationUserRepository = organizationUserRepository;
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<List<OrganizationUserDto>>> Handle(GetOrganizationUsersQuery request, CancellationToken ct)
    {
        var users = await _organizationUserRepository.GetByOrganizationIdAsync(request.OrganizationId, ct);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<UserStatus>(request.Status, true, out var status))
        {
            users = users.Where(u => u.Status == status).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            users = users.Where(u => u.Role.Equals(request.Role, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Pagination
        var pagedUsers = users
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = new List<OrganizationUserDto>();

        foreach (var user in pagedUsers)
        {
            // Get created tickets count
            var allTickets = await _ticketRepository.GetAllAsync(ct);
            var createdTicketsCount = allTickets.Count(t => t.CreatedByUserId == user.Id);

            dtos.Add(new OrganizationUserDto
            {
                Id = user.Id,
                Email = user.Email.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                Status = user.Status.ToString(),
                IsActivated = user.IsActivated,
                CreatedTicketsCount = createdTicketsCount,
                LastActivity = user.UpdatedAt ?? user.CreatedAt,
                CreatedAt = user.CreatedAt
            });
        }

        return Result.Success(dtos);
    }
}
