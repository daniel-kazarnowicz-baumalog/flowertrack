using FluentValidation;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;

namespace Flowertrack.Application.Tickets.Commands.AbandonTicket;

/// <summary>
/// Validator for AbandonTicketCommand
/// US-043: Only tickets in New status can be abandoned
/// </summary>
public sealed class AbandonTicketCommandValidator : AbstractValidator<AbandonTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;

    public AbandonTicketCommandValidator(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;

        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("Ticket ID is required")
            .MustAsync(TicketExists)
            .WithMessage("Ticket does not exist");

        RuleFor(x => x.AbandonedBy)
            .NotEmpty()
            .WithMessage("AbandonedBy user ID is required");

        RuleFor(x => x.Reason)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Reason cannot exceed 1000 characters");

        RuleFor(x => x)
            .MustAsync(CanBeAbandoned)
            .WithMessage("Only tickets in New status can be abandoned")
            .WithName("TicketId");
    }

    private async Task<bool> TicketExists(Guid ticketId, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);
        return ticket != null && !ticket.IsDeleted;
    }

    private async Task<bool> CanBeAbandoned(AbandonTicketCommand command, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null) return true; // Will be caught by TicketExists validation

        // US-043: Only New status tickets can be abandoned
        return ticket.Status == TicketStatus.New;
    }
}
