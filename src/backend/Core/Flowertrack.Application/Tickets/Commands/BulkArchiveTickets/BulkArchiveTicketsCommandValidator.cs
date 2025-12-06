using FluentValidation;

namespace Flowertrack.Application.Tickets.Commands.BulkArchiveTickets;

/// <summary>
/// Validator for BulkArchiveTicketsCommand
/// </summary>
public sealed class BulkArchiveTicketsCommandValidator : AbstractValidator<BulkArchiveTicketsCommand>
{
    private const int MaxBulkOperationSize = 100;

    public BulkArchiveTicketsCommandValidator()
    {
        RuleFor(x => x.TicketIds)
            .NotEmpty()
            .WithMessage("At least one ticket ID is required")
            .Must(ids => ids.Count <= MaxBulkOperationSize)
            .WithMessage($"Cannot process more than {MaxBulkOperationSize} tickets at once")
            .Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("All ticket IDs must be valid GUIDs");

        RuleFor(x => x.ArchivedBy)
            .NotEmpty()
            .WithMessage("ArchivedBy user ID is required");

        RuleFor(x => x.Reason)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Reason cannot exceed 1000 characters");
    }
}
