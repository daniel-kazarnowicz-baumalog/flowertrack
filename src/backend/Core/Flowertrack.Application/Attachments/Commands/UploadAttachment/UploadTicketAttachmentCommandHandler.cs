using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Attachments.Commands.UploadAttachment;

/// <summary>
/// Handler for UploadTicketAttachmentCommand
/// Uploads a file and creates an attachment record
/// </summary>
public sealed class UploadTicketAttachmentCommandHandler
    : IRequestHandler<UploadTicketAttachmentCommand, Result<Guid>>
{
    private readonly ITicketAttachmentRepository _attachmentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UploadTicketAttachmentCommandHandler> _logger;

    private const string BucketName = "ticket-attachments";

    public UploadTicketAttachmentCommandHandler(
        ITicketAttachmentRepository attachmentRepository,
        ITicketRepository ticketRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ILogger<UploadTicketAttachmentCommandHandler> logger)
    {
        _attachmentRepository = attachmentRepository;
        _ticketRepository = ticketRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        UploadTicketAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Uploading attachment to ticket {TicketId} by user {UserId}",
                request.TicketId,
                request.UploadedBy);

            // Verify ticket exists
            var ticketExists = await _ticketRepository.ExistsAsync(request.TicketId, cancellationToken);
            if (!ticketExists)
            {
                _logger.LogWarning("Ticket {TicketId} not found", request.TicketId);
                return Result.Failure<Guid>("Ticket not found");
            }

            // Generate unique storage path: tickets/{ticketId}/{guid}_{filename}
            var fileGuid = Guid.NewGuid();
            var sanitizedFileName = SanitizeFileName(request.FileName);
            var storagePath = $"tickets/{request.TicketId}/{fileGuid}_{sanitizedFileName}";

            // Upload file to storage
            var publicUrl = await _fileStorageService.UploadAsync(
                BucketName,
                storagePath,
                request.FileStream,
                request.ContentType,
                cancellationToken);

            // Create attachment entity
            var attachment = TicketAttachment.Create(
                ticketId: request.TicketId,
                uploadedBy: request.UploadedBy,
                fileName: request.FileName,
                storagePath: storagePath,
                contentType: request.ContentType,
                fileSize: request.FileSize);

            // Add attachment to repository
            await _attachmentRepository.AddAsync(attachment, cancellationToken);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully uploaded attachment {AttachmentId} to ticket {TicketId}",
                attachment.Id,
                request.TicketId);

            return Result.Success(attachment.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading attachment to ticket {TicketId}", request.TicketId);
            return Result.Failure<Guid>($"Failed to upload attachment: {ex.Message}");
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        // Remove any path characters and keep only the filename
        var name = Path.GetFileName(fileName);
        
        // Replace any invalid characters with underscores
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var c in invalidChars)
        {
            name = name.Replace(c, '_');
        }

        return name;
    }
}
