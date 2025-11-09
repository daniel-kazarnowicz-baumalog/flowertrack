using Flowertrack.Application.Machines.Commands.ActivateAlarm;
using Flowertrack.Application.Machines.Commands.ChangeMachineStatus;
using Flowertrack.Application.Machines.Commands.ClearAlarm;
using Flowertrack.Application.Machines.Commands.CompleteMaintenance;
using Flowertrack.Application.Machines.Commands.DeleteMachine;
using Flowertrack.Application.Machines.Commands.RegenerateMachineToken;
using Flowertrack.Application.Machines.Commands.RegisterMachine;
using Flowertrack.Application.Machines.Commands.ScheduleMaintenance;
using Flowertrack.Application.Machines.Commands.UpdateMachine;
using Flowertrack.Application.Machines.Queries.GetMachine;
using Flowertrack.Application.Machines.Queries.GetMachineLogs;
using Flowertrack.Application.Machines.Queries.GetMachines;
using Flowertrack.Contracts.Common;
using Flowertrack.Contracts.Machines.Requests;
using Flowertrack.Contracts.Machines.Responses;
using Flowertrack.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowertrack.Api.Controllers;

/// <summary>
/// Controller for managing machines
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MachinesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MachinesController> _logger;

    public MachinesController(
        IMediator mediator,
        ILogger<MachinesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Register a new machine
    /// US-027: Zarządzanie maszynami w organizacji
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician")]
    [ProducesResponseType(typeof(MachineResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterMachine([FromBody] RegisterMachineRequest request)
    {
        var command = new RegisterMachineCommand
        {
            OrganizationId = request.OrganizationId,
            SerialNumber = request.SerialNumber,
            Brand = request.Brand,
            Model = request.Model,
            Location = request.Location
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse(result.Error!));
        }

        var response = new MachineResponse
        {
            Id = result.Value,
            OrganizationId = request.OrganizationId,
            SerialNumber = request.SerialNumber,
            Brand = request.Brand,
            Model = request.Model,
            Location = request.Location,
            Status = "Inactive",
            CreatedAt = DateTimeOffset.UtcNow
        };

        return CreatedAtAction(
            nameof(RegisterMachine),
            new { id = result.Value },
            response);
    }

    /// <summary>
    /// Get all machines with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<MachineDetailsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMachines(
        [FromQuery] Guid? organizationId,
        [FromQuery] string? status,
        [FromQuery] string? searchTerm)
    {
        var query = new GetMachinesQuery
        {
            OrganizationId = organizationId,
            Status = status,
            SearchTerm = searchTerm
        };

        var result = await _mediator.Send(query);
        return Ok(result.Value);
    }

    /// <summary>
    /// Get machine details by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MachineDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMachine([FromRoute] Guid id)
    {
        var query = new GetMachineQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new ErrorResponse(result.Error ?? "Machine not found"));
        }

        var dto = result.Value;
        var response = new MachineDetailsResponse
        {
            Id = dto.Id,
            OrganizationId = dto.OrganizationId,
            SerialNumber = dto.SerialNumber,
            Brand = dto.Brand,
            Model = dto.Model,
            Location = dto.Location,
            Status = dto.Status,
            ApiToken = dto.ApiToken,
            LastMaintenanceDate = dto.LastMaintenanceDate,
            NextMaintenanceDate = dto.NextMaintenanceDate,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Update machine information
    /// </summary>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMachine([FromRoute] Guid id, [FromBody] UpdateMachineRequest request)
    {
        var command = new UpdateMachineCommand
        {
            MachineId = id,
            Brand = request.Brand,
            Model = request.Model,
            Location = request.Location
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return result.Error?.Contains("not found") == true
                ? NotFound(new ErrorResponse(result.Error))
                : BadRequest(new ErrorResponse(result.Error ?? "Failed to update machine"));
        }

        return NoContent();
    }

    /// <summary>
    /// Delete machine (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ServiceAdministrator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMachine([FromRoute] Guid id)
    {
        var command = new DeleteMachineCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return NotFound(new ErrorResponse(result.Error ?? "Machine not found"));
        }

        return NoContent();
    }

    /// <summary>
    /// Regenerate machine API token
    /// </summary>
    [HttpPost("{id:guid}/regenerate-token")]
    [Authorize(Roles = "ServiceAdministrator")]
    [ProducesResponseType(typeof(RegenerateMachineTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegenerateMachineToken(
        [FromRoute] Guid id,
        [FromBody] RegenerateMachineTokenRequest request)
    {
        var command = new RegenerateMachineTokenCommand(id, request.Reason);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return NotFound(new ErrorResponse(result.Error ?? "Machine not found"));
        }

        return Ok(new RegenerateMachineTokenResponse
        {
            ApiToken = result.Value,
            RegeneratedAt = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Change machine operational status
    /// </summary>
    [HttpPost("{id:guid}/change-status")]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeMachineStatus(
        [FromRoute] Guid id,
        [FromBody] ChangeMachineStatusRequest request)
    {
        if (!Enum.TryParse<MachineStatus>(request.NewStatus, true, out var status))
        {
            return BadRequest(new ErrorResponse("Invalid machine status"));
        }

        var command = new ChangeMachineStatusCommand
        {
            MachineId = id,
            NewStatus = status,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return result.Error?.Contains("not found") == true
                ? NotFound(new ErrorResponse(result.Error))
                : BadRequest(new ErrorResponse(result.Error ?? "Failed to change status"));
        }

        return NoContent();
    }

    /// <summary>
    /// Schedule maintenance for machine
    /// </summary>
    [HttpPost("{id:guid}/schedule-maintenance")]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ScheduleMaintenance(
        [FromRoute] Guid id,
        [FromBody] ScheduleMaintenanceRequest request)
    {
        var command = new ScheduleMaintenanceCommand
        {
            MachineId = id,
            ScheduledDate = request.ScheduledDate,
            IntervalDays = request.IntervalDays
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return result.Error?.Contains("not found") == true
                ? NotFound(new ErrorResponse(result.Error))
                : BadRequest(new ErrorResponse(result.Error ?? "Failed to schedule maintenance"));
        }

        return NoContent();
    }

    /// <summary>
    /// Complete maintenance for machine
    /// </summary>
    [HttpPost("{id:guid}/complete-maintenance")]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteMaintenance(
        [FromRoute] Guid id,
        [FromBody] CompleteMaintenanceRequest request)
    {
        var command = new CompleteMaintenanceCommand(id, request.CompletedDate, request.IntervalDays);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return result.Error?.Contains("not found") == true
                ? NotFound(new ErrorResponse(result.Error))
                : BadRequest(new ErrorResponse(result.Error ?? "Failed to complete maintenance"));
        }

        return NoContent();
    }

    /// <summary>
    /// Activate alarm on machine
    /// </summary>
    [HttpPost("{id:guid}/activate-alarm")]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAlarm(
        [FromRoute] Guid id,
        [FromBody] MachineAlarmRequest request)
    {
        var command = new ActivateAlarmCommand(id, request.Reason);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return NotFound(new ErrorResponse(result.Error ?? "Machine not found"));
        }

        return NoContent();
    }

    /// <summary>
    /// Clear alarm on machine
    /// </summary>
    [HttpPost("{id:guid}/clear-alarm")]
    [Authorize(Roles = "ServiceAdministrator,ServiceTechnician")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClearAlarm(
        [FromRoute] Guid id,
        [FromBody] MachineAlarmRequest request)
    {
        var command = new ClearAlarmCommand(id, request.Reason);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return result.Error?.Contains("not found") == true
                ? NotFound(new ErrorResponse(result.Error))
                : BadRequest(new ErrorResponse(result.Error ?? "Failed to clear alarm"));
        }

        return NoContent();
    }

    /// <summary>
    /// Get machine logs/history
    /// </summary>
    [HttpGet("{id:guid}/logs")]
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMachineLogs([FromRoute] Guid id)
    {
        var query = new GetMachineLogsQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(new ErrorResponse(result.Error ?? "Machine not found"));
        }

        return Ok(result.Value);
    }
}
