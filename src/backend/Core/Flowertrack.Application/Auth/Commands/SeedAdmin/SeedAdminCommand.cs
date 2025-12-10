using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Auth.Commands.SeedAdmin;

/// <summary>
/// Command to seed the initial Service Administrator user.
/// This command is intended for development/initialization purposes only.
/// </summary>
public sealed record SeedAdminCommand : IRequest<Result<SeedAdminResponse>>
{
    /// <summary>
    /// Email for the admin user
    /// </summary>
    public string Email { get; init; } = "admin@flowertrack.dev";

    /// <summary>
    /// Password for the admin user
    /// </summary>
    public string Password { get; init; } = "Admin123!";

    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; init; } = "System";

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; init; } = "Administrator";
}

/// <summary>
/// Response from SeedAdmin command
/// </summary>
public sealed record SeedAdminResponse(
    Guid UserId,
    Guid? SupabaseUserId,
    string Email,
    string FullName,
    string Message);
