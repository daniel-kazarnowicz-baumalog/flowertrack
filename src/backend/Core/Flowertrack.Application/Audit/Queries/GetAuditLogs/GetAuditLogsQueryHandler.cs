using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Audit.Queries.GetAuditLogs;

/// <summary>
/// Handler for GetAuditLogsQuery with filtering and pagination.
/// Only ServiceAdministrator users can access audit logs.
/// </summary>
public sealed class GetAuditLogsQueryHandler
    : IRequestHandler<GetAuditLogsQuery, Result<PagedResult<AuditLogDto>>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<GetAuditLogsQueryHandler> _logger;

    public GetAuditLogsQueryHandler(
        IAuditLogRepository auditLogRepository,
        ILogger<GetAuditLogsQueryHandler> logger)
    {
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResult<AuditLogDto>>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Retrieving audit logs for user {UserId} with filters: User={FilterUserId}, Action={ActionType}, Resource={ResourceType}",
                request.RequestedBy,
                request.UserId,
                request.ActionType,
                request.ResourceType);

            // Validate page parameters
            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var pageNumber = Math.Max(request.PageNumber, 1);

            // Get audit logs using the SearchAsync method with filters
            var logs = await _auditLogRepository.SearchAsync(
                userId: request.UserId,
                actionType: request.ActionType,
                resourceType: request.ResourceType,
                from: request.FromDate,
                to: request.ToDate,
                isSuccess: null, // Include both success and failure
                pageNumber: pageNumber,
                pageSize: pageSize,
                ct: cancellationToken);

            // Get total count for pagination
            var totalCount = await _auditLogRepository.GetSearchCountAsync(
                userId: request.UserId,
                actionType: request.ActionType,
                resourceType: request.ResourceType,
                from: request.FromDate,
                to: request.ToDate,
                isSuccess: null,
                ct: cancellationToken);

            // Filter by resourceId if specified (not supported in SearchAsync)
            var filteredLogs = string.IsNullOrWhiteSpace(request.ResourceId)
                ? logs
                : logs.Where(l => l.ResourceId == request.ResourceId).ToList();

            // Map to DTOs
            var dtos = filteredLogs.Select(l => new AuditLogDto
            {
                Id = l.Id,
                UserId = l.UserId,
                UserEmail = l.UserEmail,
                ActionType = l.ActionType,
                ResourceType = l.ResourceType ?? string.Empty,
                ResourceId = l.ResourceId,
                OldValue = l.OldValue,
                NewValue = l.NewValue,
                IpAddress = l.IpAddress,
                UserAgent = l.UserAgent,
                Timestamp = l.CreatedAt
            }).ToList();

            var result = new PagedResult<AuditLogDto>(
                dtos,
                pageNumber,
                pageSize,
                totalCount);

            _logger.LogInformation(
                "Retrieved {Count} audit logs (page {Page} of {TotalPages})",
                dtos.Count,
                pageNumber,
                result.TotalPages);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs");
            return Result.Failure<PagedResult<AuditLogDto>>("Failed to retrieve audit logs");
        }
    }
}
