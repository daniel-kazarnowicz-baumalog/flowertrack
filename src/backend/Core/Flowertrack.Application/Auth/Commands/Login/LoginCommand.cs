using Flowertrack.Application.Auth.DTOs;
using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Auth.Commands.Login;

/// <summary>
/// Command to authenticate a user with email and password
/// </summary>
public sealed record LoginCommand(
    string Email,
    string Password,
    string? IpAddress = null
) : IRequest<Result<LoginResponse>>;
