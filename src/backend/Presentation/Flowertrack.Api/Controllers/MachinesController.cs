using Flowertrack.Application.Machines.Commands.RegisterMachine;
using Flowertrack.Contracts.Common;
using Flowertrack.Contracts.Machines.Requests;
using Flowertrack.Contracts.Machines.Responses;
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
}
