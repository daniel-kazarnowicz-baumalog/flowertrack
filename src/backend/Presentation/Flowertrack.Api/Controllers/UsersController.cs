using Flowertrack.Application.Users.Commands.InviteServiceUser;
using Flowertrack.Application.Users.Commands.UpdateServiceUser;
using Flowertrack.Application.Users.Commands.DeactivateServiceUser;
using Flowertrack.Application.Users.Commands.ReactivateServiceUser;
using Flowertrack.Application.Users.Commands.ResetServiceUserPassword;
using Flowertrack.Application.Users.Commands.RemoveOrganizationUser;
using Flowertrack.Application.Users.Queries.GetServiceUsers;
using Flowertrack.Application.Users.Queries.GetServiceUser;
using Flowertrack.Application.Users.Queries.GetOrganizationUsers;
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

    /// <summary>
    /// Get all service users
    /// </summary>
    [HttpGet("service")]
    [ProducesResponseType(typeof(List<ServiceUserListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetServiceUsers(
        [FromQuery] string? status,
        [FromQuery] string? role,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetServiceUsersQuery(status, role, searchTerm, page, pageSize);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(new ErrorResponse(result.Error!));

        // Map to response DTO
        var response = result.Value.Select(u => new ServiceUserListItemResponse
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            FullName = u.FullName,
            PhoneNumber = u.PhoneNumber,
            Specialization = u.Specialization,
            Status = u.Status,
            IsAvailable = u.IsAvailable,
            ActiveTicketsCount = u.ActiveTicketsCount,
            LastActivity = u.LastActivity,
            CreatedAt = u.CreatedAt
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Get service user details
    /// </summary>
    [HttpGet("service/{id:guid}")]
    [ProducesResponseType(typeof(ServiceUserDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetServiceUser(Guid id)
    {
        var query = new GetServiceUserQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
                return NotFound(new ErrorResponse(result.Error));
            
            return BadRequest(new ErrorResponse(result.Error!));
        }

        var dto = result.Value;
        return Ok(new ServiceUserDetailsResponse(
            dto.Id,
            dto.Email,
            dto.FirstName,
            dto.LastName,
            dto.FullName,
            dto.PhoneNumber,
            dto.Specialization,
            dto.Status,
            dto.IsAvailable,
            dto.ActiveTicketsCount,
            dto.TotalTicketsCount,
            dto.CreatedAt,
            dto.UpdatedAt
        ));
    }

    /// <summary>
    /// Update service user
    /// </summary>
    [HttpPatch("service/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateServiceUser(Guid id, [FromBody] UpdateServiceUserRequest request)
    {
        var command = new UpdateServiceUserCommand(
            id,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Specialization
        );

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
                return NotFound(new ErrorResponse(result.Error));
            
            return BadRequest(new ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Deactivate service user
    /// </summary>
    [HttpPost("service/{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateServiceUser(Guid id, [FromBody] DeactivateServiceUserRequest request)
    {
        var command = new DeactivateServiceUserCommand(id, request.Reason);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
                return NotFound(new ErrorResponse(result.Error));
            
            return BadRequest(new ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Reactivate service user
    /// </summary>
    [HttpPost("service/{id:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ReactivateServiceUser(Guid id)
    {
        var command = new ReactivateServiceUserCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
                return NotFound(new ErrorResponse(result.Error));
            
            return BadRequest(new ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Reset service user password
    /// </summary>
    [HttpPost("service/{id:guid}/reset-password")]
    [ProducesResponseType(typeof(ResetPasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ResetServiceUserPassword(Guid id)
    {
        var command = new ResetServiceUserPasswordCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
                return NotFound(new ErrorResponse(result.Error));
            
            return BadRequest(new ErrorResponse(result.Error!));
        }

        return Ok(new ResetPasswordResponse(result.Value));
    }

    /// <summary>
    /// Get organization users
    /// </summary>
    [HttpGet("organization/{organizationId:guid}")]
    [ProducesResponseType(typeof(List<OrganizationUserListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOrganizationUsers(
        Guid organizationId,
        [FromQuery] string? status,
        [FromQuery] string? role,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetOrganizationUsersQuery(organizationId, status, role, page, pageSize);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(new ErrorResponse(result.Error!));

        // Map to response DTO
        var response = result.Value.Select(u => new OrganizationUserListItemResponse
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            FullName = u.FullName,
            PhoneNumber = u.PhoneNumber,
            Role = u.Role,
            Status = u.Status,
            IsActivated = u.IsActivated,
            CreatedTicketsCount = u.CreatedTicketsCount,
            LastActivity = u.LastActivity,
            CreatedAt = u.CreatedAt
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Get organization user details
    /// </summary>
    [HttpGet("organization/{organizationId:guid}/users/{id:guid}")]
    [ProducesResponseType(typeof(OrganizationUserDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOrganizationUser(Guid organizationId, Guid id)
    {
        // Use same query as list but find specific user
        var query = new GetOrganizationUsersQuery(organizationId, null, null, 1, 100);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(new ErrorResponse(result.Error!));

        var user = result.Value.FirstOrDefault(u => u.Id == id);
        if (user is null)
            return NotFound(new ErrorResponse("Organization user not found"));

        return Ok(new OrganizationUserDetailsResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.FullName,
            user.PhoneNumber,
            user.Role,
            user.Status,
            user.IsActivated,
            user.CreatedTicketsCount,
            user.LastActivity,
            user.CreatedAt
        ));
    }

    /// <summary>
    /// Remove organization user
    /// </summary>
    [HttpDelete("organization/{organizationId:guid}/users/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveOrganizationUser(Guid organizationId, Guid id)
    {
        var command = new RemoveOrganizationUserCommand(id, organizationId);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
                return NotFound(new ErrorResponse(result.Error));
            
            return BadRequest(new ErrorResponse(result.Error!));
        }

        return NoContent();
    }
}
