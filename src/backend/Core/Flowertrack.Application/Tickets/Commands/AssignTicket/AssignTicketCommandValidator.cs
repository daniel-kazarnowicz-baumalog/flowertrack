using FluentValidation;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;

namespace Flowertrack.Application.Tickets.Commands.AssignTicket;

/// <summary>
/// Validator for AssignTicketCommand
/// </summary>
public sealed class AssignTicketCommandValidator : AbstractValidator<AssignTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IOrganizationUserRepository _organizationUserRepository;

    public AssignTicketCommandValidator(
        ITicketRepository ticketRepository,
        IOrganizationUserRepository organizationUserRepository)
    {
        _ticketRepository = ticketRepository;
        _organizationUserRepository = organizationUserRepository;

        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required")
            .MustAsync(TicketExists)
            .WithMessage("Ticket does not exist");

        RuleFor(x => x.AssignedToUserId)
            .NotEmpty()
            .WithMessage("AssignedToUserId is required");

        RuleFor(x => x.AssignedBy)
            .NotEmpty()
            .WithMessage("AssignedBy user ID is required");

        // Validate that the ticket is not closed
        RuleFor(x => x.TicketId)
            .MustAsync(async (ticketId, cancellationToken) =>
            {
                var ticket = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);
                if (ticket == null) return true; // Will be caught by TicketExists

                return ticket.Status != TicketStatus.Closed;
            })
            .WithMessage("Cannot assign a closed ticket");

        // Validate that assignedToUserId is a member of the same organization
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
                if (ticket == null) return true; // Will be caught by TicketExists

                var organizationUsers = await _organizationUserRepository.GetByOrganizationIdAsync(
                    ticket.OrganizationId,
                    cancellationToken);

                return organizationUsers.Any(ou => ou.Id == command.AssignedToUserId);
            })
            .WithMessage("Assigned user must be a member of the ticket's organization")
            .WithName("AssignedToUserId");

        // Validate that assignedToUserId is active (not pending or disabled)
        RuleFor(x => x.AssignedToUserId)
            .MustAsync(async (userId, cancellationToken) =>
            {
                var user = await _organizationUserRepository.GetByIdAsync(userId, cancellationToken);
                if (user == null) return false;

                return user.Status == UserStatus.Active;
            })
            .WithMessage("Assigned user must be active");

        // TODO: Future enhancement - validate that assignedToUserId is a service team member
        // For now, we allow any active organization member to be assigned
        // This will be implemented when IsServiceTeamMember property is added to OrganizationUser

        // Validate that assignedBy is a member of the organization
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
                if (ticket == null) return true; // Will be caught by TicketExists

                var organizationUsers = await _organizationUserRepository.GetByOrganizationIdAsync(
                    ticket.OrganizationId,
                    cancellationToken);

                return organizationUsers.Any(ou => ou.Id == command.AssignedBy);
            })
            .WithMessage("User performing assignment must be a member of the ticket's organization")
            .WithName("AssignedBy");
    }

    private async Task<bool> TicketExists(Guid ticketId, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);
        return ticket != null && !ticket.IsDeleted;
    }
}
