using Flowertrack.Contracts.Common;

namespace Flowertrack.Contracts.Tickets.Responses;

/// <summary>
/// Response for getting ticket attachments with pagination
/// </summary>
public sealed record GetAttachmentsResponse
{
    /// <summary>
    /// List of attachments
    /// </summary>
    public List<AttachmentResponse> Attachments { get; init; } = new();

    /// <summary>
    /// Pagination information
    /// </summary>
    public PaginationMetadata Pagination { get; init; } = new();
}
