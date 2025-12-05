using FluentValidation;

namespace Flowertrack.Application.Comments.Commands.UpdateComment;

public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(v => v.CommentId)
            .NotEmpty().WithMessage("Comment ID is required.");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(5000).WithMessage("Comment content must not exceed 5000 characters.");
    }
}
