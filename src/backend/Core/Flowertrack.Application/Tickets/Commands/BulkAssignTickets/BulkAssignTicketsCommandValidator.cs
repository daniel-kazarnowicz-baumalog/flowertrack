using FluentValidation;

namespace Flowertrack.Application.Tickets.Commands.BulkAssignTickets;

/// <summary>
/// Validator for BulkAssignTicketsCommand
/// </summary>
public sealed class BulkAssignTicketsCommandValidator : AbstractValidator<BulkAssignTicketsCommand>
{
    private const int MaxBulkOperationSize = 100;

    public BulkAssignTicketsCommandValidator()
    {
        RuleFor(x => x.TicketIds)
            .NotEmpty()
            .WithMessage("At least one ticket ID is required")
            .Must(ids => ids.Count <= MaxBulkOperationSize)
            .WithMessage($"Cannot process more than {MaxBulkOperationSize} tickets at once")
            .Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("All ticket IDs must be valid GUIDs");

        RuleFor(x => x.AssignToUserId)
            .NotEmpty()
            .WithMessage("AssignToUserId is required");

        RuleFor(x => x.AssignedBy)
            .NotEmpty()
            .WithMessage("AssignedBy user ID is required");
    }
}
