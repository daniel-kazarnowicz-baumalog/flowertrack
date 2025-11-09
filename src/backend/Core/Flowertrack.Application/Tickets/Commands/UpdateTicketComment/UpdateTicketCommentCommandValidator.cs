using FluentValidation;

namespace Flowertrack.Application.Tickets.Commands.UpdateTicketComment;

/// <summary>
/// Validator for UpdateTicketCommentCommand
/// </summary>
public sealed class UpdateTicketCommentCommandValidator : AbstractValidator<UpdateTicketCommentCommand>
{
    public UpdateTicketCommentCommandValidator()
    {
        RuleFor(x => x.CommentId)
            .NotEmpty().WithMessage("Comment ID is required");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required")
            .MaximumLength(5000).WithMessage("Comment content cannot exceed 5000 characters");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}
