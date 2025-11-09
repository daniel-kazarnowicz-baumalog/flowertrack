using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Users.Queries.GetServiceUser;

public class GetServiceUserQueryHandler : IRequestHandler<GetServiceUserQuery, Result<ServiceUserDetailsDto>>
{
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly ITicketRepository _ticketRepository;

    public GetServiceUserQueryHandler(
        IServiceUserRepository serviceUserRepository,
        ITicketRepository ticketRepository)
    {
        _serviceUserRepository = serviceUserRepository;
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<ServiceUserDetailsDto>> Handle(GetServiceUserQuery request, CancellationToken ct)
    {
        var user = await _serviceUserRepository.GetByIdAsync(request.Id, ct);
        if (user is null)
            return Result.Failure<ServiceUserDetailsDto>("Service user not found");

        // Get ticket statistics
        var tickets = await _ticketRepository.GetByAssignedUserIdAsync(user.Id, ct);
        var activeTicketsCount = tickets.Count(t => 
            t.Status != TicketStatus.Closed && 
            t.Status != TicketStatus.Resolved);

        var dto = new ServiceUserDetailsDto
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
            TotalTicketsCount = tickets.Count,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return Result.Success(dto);
    }
}
