using Flowertrack.Application.Users.Commands.LoginServiceUser;
using Flowertrack.Contracts.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowertrack.Api.Controllers;

/// <summary>
/// Controller for authentication endpoints (Service Portal and Client Portal)
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Login a service user (technician or admin)
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Access token and user information</returns>
    [HttpPost("service/login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginServiceUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginServiceUser([FromBody] LoginServiceUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new LoginServiceUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess || result.Value == null)
        {
            _logger.LogWarning("Service user login failed: {Error}", result.Error);
            return Unauthorized(new { message = result.Error ?? "Invalid credentials" });
        }

        var authResult = result.Value;

        // Map to response DTO
        var response = new LoginServiceUserResponse
        {
            AccessToken = authResult.AccessToken ?? string.Empty,
            RefreshToken = authResult.RefreshToken ?? string.Empty,
            ExpiresAt = authResult.ExpiresAt ?? DateTimeOffset.UtcNow.AddHours(1),
            User = new ServiceUserDto
            {
                Id = Guid.Parse(authResult.User!.Id),
                Email = authResult.User.Email ?? string.Empty,
                FullName = authResult.Metadata?.FullName ?? string.Empty,
                Role = authResult.Metadata?.Role ?? string.Empty,
                Status = "Active" // TODO: Get from ServiceUser entity
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Logout a service user
    /// </summary>
    /// <returns>Success status</returns>
    [HttpPost("service/logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LogoutServiceUser()
    {
        // Supabase handles logout - we just return success
        // In future, we can invalidate tokens on the backend
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Send password reset email
    /// </summary>
    /// <param name="request">Email address</param>
    /// <returns>Success status</returns>
    [HttpPost("service/forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        // TODO: Implement ForgotPasswordCommand
        return Ok(new { message = "If the email exists, a password reset link has been sent" });
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("service/me")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        // TODO: Implement GetCurrentUserQuery
        return Ok(new ServiceUserDto 
        { 
            Id = Guid.NewGuid(), 
            Email = "temp@example.com",
            FullName = "Temp User",
            Role = "service_admin",
            Status = "Active"
        });
    }
}

/// <summary>
/// Temporary DTO for forgot password request
/// </summary>
public record ForgotPasswordRequest
{
    public string Email { get; init; } = string.Empty;
}
