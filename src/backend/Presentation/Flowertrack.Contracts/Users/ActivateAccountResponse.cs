namespace Flowertrack.Contracts.Users;

/// <summary>
/// Response after successful account activation
/// </summary>
public sealed record ActivateAccountResponse
{
    /// <summary>
    /// User ID
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Email address
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Full name
    /// </summary>
    public required string FullName { get; init; }

    /// <summary>
    /// Organization ID
    /// </summary>
    public required Guid OrganizationId { get; init; }

    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; init; } = "Account activated successfully. You can now log in.";
}
