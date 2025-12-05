using FluentValidation;

namespace Flowertrack.Application.Comments.Commands.AddComment;

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(v => v.TicketId)
            .NotEmpty().WithMessage("Ticket ID is required.");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(5000).WithMessage("Comment content must not exceed 5000 characters.");
    }
}
