using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Auth.DTOs;
using MediatR;

namespace Flowertrack.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string Token,
    string RefreshToken,
    string? IpAddress
) : IRequest<Result<LoginResponse>>;
