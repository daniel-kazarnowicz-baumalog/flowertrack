using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Users.Commands.DeactivateServiceUser;

public class DeactivateServiceUserCommandHandler : IRequestHandler<DeactivateServiceUserCommand, Result<Unit>>
{
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly ITicketRepository _ticketRepository;

    public DeactivateServiceUserCommandHandler(
        IServiceUserRepository serviceUserRepository,
        ITicketRepository ticketRepository)
    {
        _serviceUserRepository = serviceUserRepository;
        _ticketRepository = ticketRepository;
    }

    public async Task<Result<Unit>> Handle(DeactivateServiceUserCommand request, CancellationToken ct)
    {
        var user = await _serviceUserRepository.GetByIdAsync(request.Id, ct);
        if (user is null)
            return Result.Failure<Unit>("Service user not found");

        // Check for active tickets
        var activeTickets = await _ticketRepository.GetByAssignedUserIdAsync(request.Id, ct);
        var hasActiveTickets = activeTickets.Any(t => t.Status != Domain.Enums.TicketStatus.Closed && t.Status != Domain.Enums.TicketStatus.Resolved);
        
        if (hasActiveTickets)
            return Result.Failure<Unit>("Cannot deactivate user with active assigned tickets");

        user.Deactivate(request.Reason);

        await _serviceUserRepository.UpdateAsync(user, ct);

        return Result.Success(Unit.Value);
    }
}
