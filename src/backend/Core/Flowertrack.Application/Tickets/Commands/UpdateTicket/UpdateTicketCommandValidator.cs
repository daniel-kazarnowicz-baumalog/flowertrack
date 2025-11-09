using FluentValidation;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;

namespace Flowertrack.Application.Tickets.Commands.UpdateTicket;

/// <summary>
/// Validator for UpdateTicketCommand
/// Validates ticket existence, user permissions, and business rules
/// </summary>
public sealed class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IOrganizationUserRepository _userRepository;

    public UpdateTicketCommandValidator(
        ITicketRepository ticketRepository,
        IOrganizationUserRepository userRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;

        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("Ticket ID is required")
            .MustAsync(TicketExists)
            .WithMessage("Ticket does not exist");

        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value")
            .When(x => x.Priority.HasValue);

        RuleFor(x => x.UpdatedBy)
            .NotEmpty().WithMessage("Updated by user ID is required")
            .MustAsync(UserExists)
            .WithMessage("User does not exist");

        RuleFor(x => x)
            .MustAsync(UserHasPermissionToUpdate)
            .WithMessage("You do not have permission to update this ticket");

        RuleFor(x => x)
            .MustAsync(TicketNotClosed)
            .WithMessage("Cannot update a closed ticket. Only administrators can modify closed tickets.");
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

    private async Task<bool> UserHasPermissionToUpdate(
        UpdateTicketCommand command,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            return false;
        }

        var user = await _userRepository.GetByIdAsync(command.UpdatedBy, cancellationToken);
        if (user == null)
        {
            return false;
        }

        // User can update ticket if:
        // 1. They belong to the same organization as the ticket
        // 2. OR they are a service team member (TODO: implement when service team is ready)
        // 3. OR they are the ticket creator
        return user.OrganizationId == ticket.OrganizationId || 
               user.Id == ticket.CreatedByUserId;
    }

    private async Task<bool> TicketNotClosed(
        UpdateTicketCommand command,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            return true; // Will be caught by TicketExists validation
        }

        // Check if ticket is closed
        // TODO: Add admin override when admin/role system is implemented
        return ticket.Status != TicketStatus.Closed;
    }
}
