using Flowertrack.Application.Auth.Commands.Login;
using Flowertrack.Application.Auth.Commands.RefreshToken;
using Flowertrack.Application.Users.Commands.ActivateOrganizationUser;
using Flowertrack.Application.Users.Commands.ForgotPassword;
using Flowertrack.Application.Users.Commands.InviteOrganizationUser;
using Flowertrack.Application.Users.Commands.LoginOrganizationUser;
using Flowertrack.Application.Users.Commands.LoginServiceUser;
using Flowertrack.Application.Users.Commands.ResetPassword;
using Flowertrack.Application.Users.Commands.SignupServiceUser;
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

    #region Service Portal Authentication

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
    /// Create/signup a new service user account (admin only)
    /// </summary>
    /// <param name="request">User creation details</param>
    /// <returns>Created user information</returns>
    [HttpPost("service/signup")]
    [Authorize(Policy = "RequireServiceAdmin")]
    [ProducesResponseType(typeof(SignupServiceUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignupServiceUser([FromBody] SignupServiceUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new SignupServiceUserCommand
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Specialization = request.Specialization,
            Password = request.Password
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess || result.Value == null)
        {
            _logger.LogWarning("Service user signup failed: {Error}", result.Error);
            return BadRequest(new { message = result.Error ?? "Failed to create user account" });
        }

        var signupResult = result.Value;

        var response = new SignupServiceUserResponse
        {
            UserId = signupResult.UserId,
            Email = signupResult.Email,
            FullName = signupResult.FullName,
            ActivationToken = signupResult.ActivationToken,
            ActivationTokenExpiresAt = signupResult.ActivationTokenExpiresAt
        };

        return CreatedAtAction(
            nameof(GetCurrentUser),
            new { id = signupResult.UserId },
            response
        );
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new ForgotPasswordCommand
        {
            Email = request.Email
        };

        var result = await _mediator.Send(command);

        // Always return success for security reasons (don't reveal if email exists)
        return Ok(new { message = "If the email exists, a password reset link has been sent" });
    }

    /// <summary>
    /// Reset password with token from email
    /// </summary>
    /// <param name="request">New password and reset token</param>
    /// <returns>Success status</returns>
    [HttpPost("service/reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new ResetPasswordCommand
        {
            NewPassword = request.NewPassword,
            Token = request.Token
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error ?? "Failed to reset password" });
        }

        return Ok(new { message = "Password reset successfully" });
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

    #endregion

    #region Client Portal Authentication

    /// <summary>
    /// Login an organization user (operator or admin)
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Access token and user information</returns>
    [HttpPost("client/login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginOrganizationUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginOrganizationUser([FromBody] LoginOrganizationUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new LoginOrganizationUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess || result.Value == null)
        {
            _logger.LogWarning("Organization user login failed: {Error}", result.Error);
            return Unauthorized(new { message = result.Error ?? "Invalid credentials or account not activated" });
        }

        var loginResult = result.Value;

        var response = new LoginOrganizationUserResponse
        {
            AccessToken = loginResult.AccessToken,
            RefreshToken = loginResult.RefreshToken,
            ExpiresAt = loginResult.ExpiresAt,
            User = new OrganizationUserDto
            {
                Id = loginResult.UserId,
                OrganizationId = loginResult.OrganizationId,
                OrganizationName = loginResult.OrganizationName,
                Email = loginResult.Email,
                FullName = loginResult.FullName,
                Role = loginResult.Role,
                Status = loginResult.Status,
                IsActivated = loginResult.IsActivated
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Invite a new organization user (organization admin or service admin only)
    /// </summary>
    /// <param name="request">User details</param>
    /// <returns>Created user information with invitation token</returns>
    [HttpPost("client/invite")]
    [Authorize(Policy = "RequireOrganizationAdmin")] // Or RequireServiceAdmin
    [ProducesResponseType(typeof(InviteOrganizationUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> InviteOrganizationUser([FromBody] InviteOrganizationUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new InviteOrganizationUserCommand
        {
            OrganizationId = request.OrganizationId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess || result.Value == null)
        {
            _logger.LogWarning("Organization user invitation failed: {Error}", result.Error);
            return BadRequest(new { message = result.Error ?? "Failed to invite user" });
        }

        var inviteResult = result.Value;

        var response = new InviteOrganizationUserResponse
        {
            UserId = inviteResult.UserId,
            Email = inviteResult.Email,
            FullName = inviteResult.FullName,
            InvitationToken = inviteResult.InvitationToken,
            InvitationTokenExpiresAt = inviteResult.InvitationTokenExpiresAt
        };

        return CreatedAtAction(
            nameof(GetCurrentUser), // TODO: Create GetOrganizationUser endpoint
            new { id = inviteResult.UserId },
            response
        );
    }

    /// <summary>
    /// Activate organization user account with invitation token
    /// </summary>
    /// <param name="request">Activation token and optional password</param>
    /// <returns>Activation confirmation</returns>
    [HttpPost("client/activate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ActivateAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActivateAccount([FromBody] ActivateAccountRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new ActivateOrganizationUserCommand
        {
            Token = request.Token,
            Password = request.Password
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess || result.Value == null)
        {
            _logger.LogWarning("Account activation failed: {Error}", result.Error);
            return BadRequest(new { message = result.Error ?? "Activation failed" });
        }

        var activationResult = result.Value;

        var response = new ActivateAccountResponse
        {
            UserId = activationResult.UserId,
            Email = activationResult.Email,
            FullName = activationResult.FullName,
            OrganizationId = activationResult.OrganizationId,
            Message = activationResult.Message
        };

        return Ok(response);
    }

    /// <summary>
    /// Logout an organization user
    /// </summary>
    /// <returns>Success status</returns>
    [HttpPost("client/logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LogoutOrganizationUser()
    {
        // Supabase handles logout - we just return success
        // In future, we can invalidate tokens on the backend
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Get current authenticated organization user information
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("client/me")]
    [Authorize]
    [ProducesResponseType(typeof(OrganizationUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentOrganizationUser()
    {
        // TODO: Implement GetCurrentOrganizationUserQuery
        return Ok(new OrganizationUserDto 
        { 
            Id = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            OrganizationName = "Example Organization",
            Email = "temp@organization.com",
            FullName = "Temp User",
            Role = "organization_admin",
            Status = "Active",
            IsActivated = true
        });
    }

    #endregion

    #region Unified Authentication (Phase A)

    /// <summary>
    /// Unified login endpoint for both service users and organization users
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token, refresh token, and user information</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Application.Auth.DTOs.LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Unified login attempt for email: {Email}", request.Email);

        var command = new LoginCommand(request.Email, request.Password, GetClientIpAddress());
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning("Login failed for email: {Email}, Reason: {Error}", request.Email, result.Error);
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        _logger.LogInformation("Login successful for email: {Email}", request.Email);
        return Ok(result.Value);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Access token and refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New access token and refresh token</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Application.Auth.DTOs.LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Refresh token attempt");

        var command = new RefreshTokenCommand(
            request.AccessToken,
            request.RefreshToken,
            GetClientIpAddress()
        );

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning("Refresh token failed: {Error}", result.Error);
            return Unauthorized(new ProblemDetails
            {
                Title = "Token refresh failed",
                Detail = result.Error,
                Status = StatusCodes.Status401Unauthorized
            });
        }

        _logger.LogInformation("Refresh token successful");
        return Ok(result.Value);
    }

    /// <summary>
    /// Get client IP address from request
    /// </summary>
    private string? GetClientIpAddress()
    {
        // Try X-Forwarded-For header first (for reverse proxies)
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var ip = forwardedFor.ToString().Split(',').FirstOrDefault()?.Trim();
            if (!string.IsNullOrEmpty(ip))
            {
                return ip;
            }
        }

        // Try X-Real-IP header
        if (Request.Headers.TryGetValue("X-Real-IP", out var realIp))
        {
            var ip = realIp.ToString();
            if (!string.IsNullOrEmpty(ip))
            {
                return ip;
            }
        }

        // Fall back to RemoteIpAddress
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    #endregion
}

/// <summary>
/// Login request DTO for unified authentication
/// </summary>
public sealed record LoginRequest(string Email, string Password);

/// <summary>
/// Refresh token request DTO
/// </summary>
public sealed record RefreshTokenRequest(string AccessToken, string RefreshToken);
