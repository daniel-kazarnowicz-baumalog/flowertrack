using FluentValidation;

namespace Flowertrack.Application.Tickets.Commands.UploadTicketAttachment;

/// <summary>
/// Validator for UploadTicketAttachmentCommand
/// </summary>
public sealed class UploadTicketAttachmentCommandValidator : AbstractValidator<UploadTicketAttachmentCommand>
{
    private const long MaxFileSizeBytes = 52428800; // 50 MB
    private static readonly string[] AllowedContentTypes = 
    {
        "application/pdf",
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // docx
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // xlsx
        "text/plain"
    };

    public UploadTicketAttachmentCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("Ticket ID is required");

        RuleFor(x => x.UploadedBy)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required")
            .MaximumLength(255).WithMessage("File name cannot exceed 255 characters");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("File size must be greater than zero")
            .LessThanOrEqualTo(MaxFileSizeBytes).WithMessage("File size cannot exceed 50 MB");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required")
            .Must(BeAllowedContentType).WithMessage("File type is not allowed. Allowed types: PDF, JPG, PNG, GIF, DOCX, XLSX, TXT");

        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("File stream is required");
    }

    private bool BeAllowedContentType(string contentType)
    {
        return AllowedContentTypes.Contains(contentType.ToLowerInvariant());
    }
}
