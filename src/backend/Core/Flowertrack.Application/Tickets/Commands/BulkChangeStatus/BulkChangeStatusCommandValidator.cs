using FluentValidation;
using Flowertrack.Domain.Enums;

namespace Flowertrack.Application.Tickets.Commands.BulkChangeStatus;

/// <summary>
/// Validator for BulkChangeStatusCommand
/// </summary>
public sealed class BulkChangeStatusCommandValidator : AbstractValidator<BulkChangeStatusCommand>
{
    private const int MaxBulkOperationSize = 100;

    public BulkChangeStatusCommandValidator()
    {
        RuleFor(x => x.TicketIds)
            .NotEmpty()
            .WithMessage("At least one ticket ID is required")
            .Must(ids => ids.Count <= MaxBulkOperationSize)
            .WithMessage($"Cannot process more than {MaxBulkOperationSize} tickets at once")
            .Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("All ticket IDs must be valid GUIDs");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid ticket status");

        RuleFor(x => x.ChangedBy)
            .NotEmpty()
            .WithMessage("ChangedBy user ID is required");

        // Reason is required for Resolved and Closed statuses with minimum 10 characters
        RuleFor(x => x.Reason)
            .NotEmpty()
            .When(x => x.NewStatus == TicketStatus.Resolved || x.NewStatus == TicketStatus.Closed)
            .WithMessage("Reason is required when resolving or closing tickets");

        RuleFor(x => x.Reason)
            .MinimumLength(10)
            .When(x => x.NewStatus == TicketStatus.Resolved || x.NewStatus == TicketStatus.Closed)
            .WithMessage("Reason must be at least 10 characters when resolving or closing tickets");

        RuleFor(x => x.Reason)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Reason cannot exceed 1000 characters");
    }
}
