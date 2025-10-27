using Flowertrack.Application.Users.Commands.InviteServiceUser;
using Flowertrack.Contracts.Common;
using Flowertrack.Contracts.Users.Requests;
using Flowertrack.Contracts.Users.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowertrack.Api.Controllers;

/// <summary>
/// Controller for managing users
/// </summary>
[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "ServiceAdministrator")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IMediator mediator,
        ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Invite a new service user
    /// US-032: Dodawanie nowego serwisanta
    /// </summary>
    [HttpPost("service/invite")]
    [ProducesResponseType(typeof(InvitationResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> InviteServiceUser([FromBody] InviteServiceUserRequest request)
    {
        var command = new InviteServiceUserCommand
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Specialization = request.Specialization
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse(result.Error!));
        }

        return Accepted(new InvitationResponse
        {
            UserId = result.Value,
            Email = request.Email,
            Message = "Invitation email sent to service user",
            InvitationValidUntil = DateTimeOffset.UtcNow.AddDays(7)
        });
    }
}
