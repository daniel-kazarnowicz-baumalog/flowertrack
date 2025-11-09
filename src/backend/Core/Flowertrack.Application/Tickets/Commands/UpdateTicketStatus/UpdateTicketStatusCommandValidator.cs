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

        var currentStatus = ticket.Status;
        var newStatus = command.NewStatus;

        // Cannot transition from same status to same status
        if (currentStatus == newStatus)
        {
            return false;
        }

        // Define valid transitions
        var validTransitions = new Dictionary<TicketStatus, List<TicketStatus>>
        {
            { TicketStatus.New, new List<TicketStatus> { TicketStatus.Accepted, TicketStatus.Closed } },
            { TicketStatus.Accepted, new List<TicketStatus> { TicketStatus.InProgress, TicketStatus.Closed } },
            { TicketStatus.InProgress, new List<TicketStatus> { TicketStatus.Resolved, TicketStatus.Closed } },
            { TicketStatus.Resolved, new List<TicketStatus> { TicketStatus.Closed, TicketStatus.Reopened } },
            { TicketStatus.Reopened, new List<TicketStatus> { TicketStatus.InProgress, TicketStatus.Resolved, TicketStatus.Closed } },
            { TicketStatus.Closed, new List<TicketStatus>() } // Closed is final state, no transitions allowed
        };

        if (!validTransitions.ContainsKey(currentStatus))
        {
            return false;
        }

        var allowedTransitions = validTransitions[currentStatus];
        
        // Check if transition is allowed
        if (!allowedTransitions.Contains(newStatus))
        {
            return false;
        }

        // Special rule: Resolved → Reopened only within 14 days
        if (currentStatus == TicketStatus.Resolved && newStatus == TicketStatus.Reopened)
        {
            if (ticket.ResolvedAt.HasValue)
            {
                var daysSinceResolved = (DateTimeOffset.UtcNow - ticket.ResolvedAt.Value).TotalDays;
                if (daysSinceResolved > 14)
                {
                    return false; // Cannot reopen after 14 days
                }
            }
            else
            {
                return false; // Ticket was never properly resolved
            }
        }

        return true;
    }
}
