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
    private readonly IServiceUserRepository _serviceUserRepository;

    public UpdateTicketStatusCommandValidator(
        ITicketRepository ticketRepository,
        IOrganizationUserRepository organizationUserRepository,
        IServiceUserRepository serviceUserRepository)
    {
        _ticketRepository = ticketRepository;
        _organizationUserRepository = organizationUserRepository;
        _serviceUserRepository = serviceUserRepository;

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

        // Reason is required for Resolved and Closed statuses with minimum 10 characters
        RuleFor(x => x.Reason)
            .NotEmpty()
            .When(x => x.NewStatus == TicketStatus.Resolved || x.NewStatus == TicketStatus.Closed)
            .WithMessage("Reason is required when resolving or closing a ticket");

        RuleFor(x => x.Reason)
            .MinimumLength(10)
            .When(x => x.NewStatus == TicketStatus.Resolved || x.NewStatus == TicketStatus.Closed)
            .WithMessage("Reason must be at least 10 characters when resolving or closing a ticket");

        RuleFor(x => x.Reason)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Reason cannot exceed 1000 characters");

        // User must be a service user OR a member of the organization
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                // First check if user is a service user (technician)
                var serviceUser = await _serviceUserRepository.GetByIdAsync(command.ChangedBy, cancellationToken);
                if (serviceUser != null)
                {
                    return true; // Service users can update ticket status
                }

                // If not a service user, check if user is member of the ticket's organization
                var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
                if (ticket == null) return true; // Will be caught by TicketExists validation

                var organizationUsers = await _organizationUserRepository.GetByOrganizationIdAsync(
                    ticket.OrganizationId,
                    cancellationToken);

                return organizationUsers.Any(ou => ou.Id == command.ChangedBy);
            })
            .WithMessage("User is not authorized to update this ticket's status")
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
