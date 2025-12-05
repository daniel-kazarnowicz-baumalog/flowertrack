using FluentValidation;
using Flowertrack.Domain.Repositories;

namespace Flowertrack.Application.Tickets.Commands.AddTicketComment;

/// <summary>
/// Validator for AddTicketCommentCommand
/// </summary>
public sealed class AddTicketCommentCommandValidator : AbstractValidator<AddTicketCommentCommand>
{
    private readonly ITicketRepository _ticketRepository;

    public AddTicketCommentCommandValidator(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;

        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("Ticket ID is required");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required")
            .MaximumLength(5000).WithMessage("Comment content cannot exceed 5000 characters");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        // Note: Business rule validation for internal comments (only service users can add)
        // is handled in the controller/authorization layer
    }
}
