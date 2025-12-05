using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Tickets.Commands.UploadTicketAttachment;
using Flowertrack.Application.Tickets.Commands.DeleteTicketAttachment;
using Flowertrack.Application.Tickets.Queries.GetTicketAttachments;
using Flowertrack.Contracts.Common;
using Flowertrack.Contracts.Tickets.Responses;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flowertrack.Api.Controllers;

/// <summary>
/// Controller for managing ticket attachments
/// </summary>
[ApiController]
[Route("api/tickets/{ticketId:guid}/attachments")]
[Authorize(Policy = "RequireAuthenticatedUser")]
public class TicketAttachmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorageService;
    private readonly ITicketAttachmentRepository _attachmentRepository;
    private readonly ILogger<TicketAttachmentsController> _logger;

    private const string BucketName = "ticket-attachments";

    public TicketAttachmentsController(
        IMediator mediator,
        IFileStorageService fileStorageService,
        ITicketAttachmentRepository attachmentRepository,
        ILogger<TicketAttachmentsController> logger)
    {
        _mediator = mediator;
        _fileStorageService = fileStorageService;
        _attachmentRepository = attachmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get attachments for a ticket
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 50, max 100)</param>
    /// <returns>Paginated list of attachments</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetAttachmentsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAttachments(
        Guid ticketId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        var query = new GetTicketAttachmentsQuery
        {
            TicketId = ticketId,
            RequestedBy = userId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to get attachments for ticket {TicketId}: {Error}", ticketId, result.Error);
            return result.Error == "Ticket not found" 
                ? NotFound(new ErrorResponse(result.Error)) 
                : BadRequest(new ErrorResponse(result.Error ?? "Failed to get attachments"));
        }

        var response = new GetAttachmentsResponse
        {
            Attachments = result.Value!.Items.Select(a => new AttachmentResponse
            {
                Id = a.Id,
                TicketId = a.TicketId,
                FileName = a.FileName,
                ContentType = a.ContentType,
                FileSizeBytes = a.FileSizeBytes,
                UploadedBy = a.UploadedBy,
                UploadedByName = a.UploadedByName,
                CreatedAt = a.CreatedAt,
                DownloadUrl = Url.Action(nameof(DownloadAttachment), new { ticketId, attachmentId = a.Id }) ?? ""
            }).ToList(),
            Pagination = new PaginationMetadata
            {
                PageNumber = result.Value.PageNumber,
                PageSize = result.Value.PageSize,
                TotalPages = result.Value.TotalPages,
                TotalCount = result.Value.TotalCount,
                HasPreviousPage = result.Value.PageNumber > 1,
                HasNextPage = result.Value.PageNumber < result.Value.TotalPages
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Upload an attachment to a ticket
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="file">File to upload</param>
    /// <returns>Created attachment ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [RequestSizeLimit(52428800)] // 50 MB limit
    public async Task<IActionResult> UploadAttachment(
        Guid ticketId,
        IFormFile file)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new ErrorResponse("No file provided"));
        }

        await using var stream = file.OpenReadStream();

        var command = new UploadTicketAttachmentCommand
        {
            TicketId = ticketId,
            UploadedBy = userId,
            FileName = file.FileName,
            FileStream = stream,
            ContentType = file.ContentType,
            FileSize = file.Length
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to upload attachment to ticket {TicketId}: {Error}", ticketId, result.Error);
            return result.Error == "Ticket not found" 
                ? NotFound(new ErrorResponse(result.Error)) 
                : BadRequest(new ErrorResponse(result.Error ?? "Failed to upload attachment"));
        }

        return CreatedAtAction(
            nameof(GetAttachments),
            new { ticketId },
            result.Value);
    }

    /// <summary>
    /// Download an attachment
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="attachmentId">Attachment ID</param>
    /// <returns>File stream</returns>
    [HttpGet("{attachmentId:guid}/download")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadAttachment(
        Guid ticketId,
        Guid attachmentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        // Get attachment metadata
        var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
        if (attachment == null || attachment.TicketId != ticketId)
        {
            return NotFound(new ErrorResponse("Attachment not found"));
        }

        try
        {
            // Download file from storage
            var fileBytes = await _fileStorageService.DownloadAsync(
                BucketName,
                attachment.StoragePath);

            // Return file with proper content type and disposition
            return File(
                fileBytes,
                attachment.ContentType,
                attachment.FileName);
        }
        catch (FileNotFoundException)
        {
            _logger.LogError("File not found in storage: {StoragePath}", attachment.StoragePath);
            return NotFound(new ErrorResponse("File not found in storage"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading attachment {AttachmentId}", attachmentId);
            return StatusCode(500, new ErrorResponse("Failed to download file"));
        }
    }

    /// <summary>
    /// Delete an attachment from a ticket
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="attachmentId">Attachment ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{attachmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAttachment(
        Guid ticketId,
        Guid attachmentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("User authentication required"));
        }

        var command = new DeleteTicketAttachmentCommand
        {
            AttachmentId = attachmentId,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to delete attachment {AttachmentId}: {Error}", attachmentId, result.Error);
            return result.Error == "Attachment not found" 
                ? NotFound(new ErrorResponse(result.Error)) 
                : BadRequest(new ErrorResponse(result.Error ?? "Failed to delete attachment"));
        }

        return NoContent();
    }
}
