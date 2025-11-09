namespace Flowertrack.Application.Common.Models;

/// <summary>
/// Result of an authentication operation
/// </summary>
public class AuthResult
{
    /// <summary>
    /// Indicates if the authentication was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Access token (JWT) for authenticated requests
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Supabase user object
    /// </summary>
    public Supabase.Gotrue.User? User { get; set; }

    /// <summary>
    /// Token expiration timestamp
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// Error message if authentication failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// User metadata extracted from the user object
    /// </summary>
    public UserMetadata? Metadata => User?.UserMetadata != null 
        ? UserMetadata.FromDictionary(User.UserMetadata) 
        : null;

    /// <summary>
    /// Creates a successful auth result
    /// </summary>
    public static AuthResult CreateSuccess(
        string accessToken, 
        string refreshToken, 
        Supabase.Gotrue.User user, 
        DateTimeOffset expiresAt)
    {
        return new AuthResult
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = user,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Creates a failed auth result
    /// </summary>
    public static AuthResult CreateFailure(string errorMessage)
    {
        return new AuthResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
