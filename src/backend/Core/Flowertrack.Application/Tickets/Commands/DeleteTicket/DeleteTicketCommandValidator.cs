using FluentValidation;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;

namespace Flowertrack.Application.Tickets.Commands.DeleteTicket;

/// <summary>
/// Validator for DeleteTicketCommand
/// Validates ticket existence, user permissions, and deletion constraints
/// </summary>
public sealed class DeleteTicketCommandValidator : AbstractValidator<DeleteTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IOrganizationUserRepository _userRepository;

    public DeleteTicketCommandValidator(
        ITicketRepository ticketRepository,
        IOrganizationUserRepository userRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;

        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("Ticket ID is required")
            .MustAsync(TicketExists)
            .WithMessage("Ticket does not exist");

        RuleFor(x => x.DeletedBy)
            .NotEmpty().WithMessage("Deleted by user ID is required")
            .MustAsync(UserExists)
            .WithMessage("User does not exist");

        RuleFor(x => x)
            .MustAsync(UserHasPermissionToDelete)
            .WithMessage("You do not have permission to delete this ticket");

        RuleFor(x => x)
            .MustAsync(TicketCanBeDeleted)
            .WithMessage("Ticket cannot be deleted in its current state. Only tickets in 'New' or 'Closed' status can be deleted.");
    }

    private async Task<bool> TicketExists(Guid ticketId, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);
        return ticket != null;
    }

    private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user != null;
    }

    private async Task<bool> UserHasPermissionToDelete(
        DeleteTicketCommand command,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            return false;
        }

        var user = await _userRepository.GetByIdAsync(command.DeletedBy, cancellationToken);
        if (user == null)
        {
            return false;
        }

        // User can delete ticket if:
        // 1. They are the ticket creator
        // 2. They belong to the same organization and have admin role
        // 3. OR they are a service team member (TODO: implement when service team is ready)
        
        var isCreator = user.Id == ticket.CreatedByUserId;
        var isSameOrganization = user.OrganizationId == ticket.OrganizationId;
        var isAdmin = user.Role == OrganizationUserRole.Admin || user.Role == OrganizationUserRole.Owner;

        return isCreator || (isSameOrganization && isAdmin);
    }

    private async Task<bool> TicketCanBeDeleted(
        DeleteTicketCommand command,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            return true; // Will be caught by TicketExists validation
        }

        // Only allow deletion of tickets in New or Closed status
        // This prevents deletion of tickets that are actively being worked on
        return ticket.Status == TicketStatus.New || 
               ticket.Status == TicketStatus.Closed;
    }
}
