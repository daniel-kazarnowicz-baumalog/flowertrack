using Flowertrack.Api.Middleware;
using Flowertrack.Application.Machines.Commands.IngestMachineLog;
using Flowertrack.Contracts.Common;
using Flowertrack.Contracts.Machines.Requests;
using Flowertrack.Contracts.Machines.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flowertrack.Api.Controllers;

/// <summary>
/// Controller for machine log ingestion endpoints.
/// These endpoints use machine API token authentication (X-API-Token header)
/// instead of standard JWT authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class IngestController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<IngestController> _logger;

    public IngestController(
        IMediator mediator,
        ILogger<IngestController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Ingest a machine log entry
    /// US-050: Zbieranie logów maszyn poprzez API
    /// </summary>
    /// <remarks>
    /// This endpoint requires authentication using the X-API-Token header.
    /// The token must be a valid machine API token.
    /// </remarks>
    [HttpPost("logs")]
    [ProducesResponseType(typeof(IngestMachineLogResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> IngestLog([FromBody] IngestMachineLogRequest request)
    {
        // Get the authenticated machine ID from the middleware
        if (!HttpContext.Items.TryGetValue(MachineTokenAuthenticationMiddleware.MachineIdContextKey, out var machineIdObj) ||
            machineIdObj is not Guid machineId)
        {
            _logger.LogError("Machine ID not found in HttpContext after authentication middleware");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(
                "Authentication error",
                "Machine authentication context not available"));
        }

        var command = new IngestMachineLogCommand
        {
            MachineId = machineId,
            LogType = request.LogType,
            LogContent = request.LogContent,
            Status = request.Status,
            AlarmCode = request.AlarmCode,
            Severity = request.Severity
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Failed to ingest log for machine {MachineId}: {Error}",
                machineId,
                result.Error);

            return BadRequest(new ErrorResponse(result.Error!));
        }

        var response = new IngestMachineLogResponse
        {
            Id = result.Value,
            MachineId = machineId,
            ReceivedAt = DateTimeOffset.UtcNow,
            LogType = request.LogType,
            StatusUpdated = request.LogType.Equals("ALARM", StringComparison.OrdinalIgnoreCase)
        };

        _logger.LogInformation(
            "Successfully ingested {LogType} log for machine {MachineId}, log ID: {LogId}",
            request.LogType,
            machineId,
            result.Value);

        return CreatedAtAction(nameof(IngestLog), new { id = result.Value }, response);
    }
}
