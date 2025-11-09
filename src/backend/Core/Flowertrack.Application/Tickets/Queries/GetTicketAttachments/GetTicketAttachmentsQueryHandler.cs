using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Queries.GetTicketAttachments;

/// <summary>
/// Handler for GetTicketAttachmentsQuery
/// Retrieves paginated attachments for a ticket
/// </summary>
public sealed class GetTicketAttachmentsQueryHandler
    : IRequestHandler<GetTicketAttachmentsQuery, Result<PagedResult<AttachmentDto>>>
{
    private readonly ITicketAttachmentRepository _attachmentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly ILogger<GetTicketAttachmentsQueryHandler> _logger;

    public GetTicketAttachmentsQueryHandler(
        ITicketAttachmentRepository attachmentRepository,
        ITicketRepository ticketRepository,
        IServiceUserRepository serviceUserRepository,
        IOrganizationUserRepository organizationUserRepository,
        ILogger<GetTicketAttachmentsQueryHandler> logger)
    {
        _attachmentRepository = attachmentRepository;
        _ticketRepository = ticketRepository;
        _serviceUserRepository = serviceUserRepository;
        _organizationUserRepository = organizationUserRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResult<AttachmentDto>>> Handle(
        GetTicketAttachmentsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Getting attachments for ticket {TicketId}, page {PageNumber}",
                request.TicketId,
                request.PageNumber);

            // Verify ticket exists
            var ticketExists = await _ticketRepository.ExistsAsync(request.TicketId, cancellationToken);
            if (!ticketExists)
            {
                _logger.LogWarning("Ticket {TicketId} not found", request.TicketId);
                return Result.Failure<PagedResult<AttachmentDto>>("Ticket not found");
            }

            // Validate page size
            var pageSize = Math.Min(request.PageSize, 100);

            // Get attachments from repository
            var (attachments, totalCount) = await _attachmentRepository.GetByTicketIdAsync(
                request.TicketId,
                request.PageNumber,
                pageSize,
                cancellationToken);

            // Map to DTOs with user names
            var attachmentDtos = new List<AttachmentDto>();
            foreach (var attachment in attachments)
            {
                var uploaderName = await GetUserNameAsync(attachment.UploadedBy, cancellationToken);
                
                attachmentDtos.Add(new AttachmentDto
                {
                    Id = attachment.Id,
                    TicketId = attachment.TicketId,
                    FileName = attachment.FileName,
                    ContentType = attachment.ContentType,
                    FileSizeBytes = attachment.FileSizeBytes,
                    UploadedBy = attachment.UploadedBy,
                    UploadedByName = uploaderName,
                    CreatedAt = attachment.CreatedAt,
                    StoragePath = attachment.StoragePath
                });
            }

            var pagedResult = new PagedResult<AttachmentDto>(
                attachmentDtos,
                request.PageNumber,
                pageSize,
                totalCount);

            return Result.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attachments for ticket {TicketId}", request.TicketId);
            return Result.Failure<PagedResult<AttachmentDto>>($"Failed to get attachments: {ex.Message}");
        }
    }

    private async Task<string> GetUserNameAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Try to find as service user first
        var serviceUser = await _serviceUserRepository.GetByIdAsync(userId, cancellationToken);
        if (serviceUser != null)
        {
            return $"{serviceUser.FirstName} {serviceUser.LastName}";
        }

        // Try to find as organization user
        var orgUser = await _organizationUserRepository.GetByIdAsync(userId, cancellationToken);
        if (orgUser != null)
        {
            return $"{orgUser.FirstName} {orgUser.LastName}";
        }

        return "Unknown User";
    }
}
