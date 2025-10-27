namespace Flowertrack.Contracts.Organizations.Responses;

/// <summary>
/// Response for successful organization onboarding
/// </summary>
public sealed record OnboardingConfirmationResponse
{
    public Guid OrganizationId { get; init; }
    public string Message { get; init; } = string.Empty;
    public DateTimeOffset InvitationValidUntil { get; init; }
}
