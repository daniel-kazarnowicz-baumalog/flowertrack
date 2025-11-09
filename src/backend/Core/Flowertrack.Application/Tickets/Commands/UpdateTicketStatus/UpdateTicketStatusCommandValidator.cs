using FluentValidation;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;

namespace Flowertrack.Application.Tickets.Commands.UpdateTicketStatus;

/// <summary>
/// Validator for UpdateTicketStatusCommand with comprehensive state transition rules
/// </summary>
public sealed class UpdateTicketStatusCommandValidator : AbstractValidator<UpdateTicketStatusCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IOrganizationUserRepository _organizationUserRepository;

    public UpdateTicketStatusCommandValidator(
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

        RuleFor(x => x.ChangedBy)
            .NotEmpty()
            .WithMessage("ChangedBy user ID is required");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid ticket status");

        // Reason is required for Resolved and Closed statuses
        RuleFor(x => x.Reason)
            .NotEmpty()
            .When(x => x.NewStatus == TicketStatus.Resolved || x.NewStatus == TicketStatus.Closed)
            .WithMessage("Reason is required when resolving or closing a ticket");

        RuleFor(x => x.Reason)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Reason cannot exceed 1000 characters");

        // User must be member of the organization
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
                if (ticket == null) return true; // Will be caught by TicketExists validation

                var organizationUsers = await _organizationUserRepository.GetByOrganizationIdAsync(
                    ticket.OrganizationId,
                    cancellationToken);

                return organizationUsers.Any(ou => ou.Id == command.ChangedBy);
            })
            .WithMessage("User is not a member of the ticket's organization")
            .WithName("ChangedBy");

        // State transition validation
        RuleFor(x => x)
            .MustAsync(IsValidStateTransition)
            .WithMessage("Invalid state transition")
            .WithName("NewStatus");
    }

    private async Task<bool> TicketExists(Guid ticketId, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);
        return ticket != null && !ticket.IsDeleted;
    }

    private async Task<bool> IsValidStateTransition(
        UpdateTicketStatusCommand command,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null) return true; // Will be caught by TicketExists validation

        // Delegate transition validation to the domain entity
        return ticket.IsValidStatusTransition(command.NewStatus, DateTimeOffset.UtcNow);
    }
}
