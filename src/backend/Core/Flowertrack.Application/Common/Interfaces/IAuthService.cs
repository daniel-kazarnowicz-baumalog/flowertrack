using Flowertrack.Application.Common.Models;

namespace Flowertrack.Application.Common.Interfaces;

/// <summary>
/// Service for managing user authentication through Supabase Auth
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user with Supabase Auth
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="password">User's password</param>
    /// <param name="metadata">Additional user metadata (role, full name, organization ID)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result containing tokens and user info</returns>
    Task<AuthResult> SignUpAsync(string email, string password, UserMetadata metadata, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="password">User's password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result containing tokens and user info</returns>
    Task<AuthResult> SignInAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs out a user by invalidating their access token
    /// </summary>
    /// <param name="accessToken">User's access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if sign out was successful</returns>
    Task<bool> SignOutAsync(string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a password reset email to the user
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if email was sent successfully</returns>
    Task<bool> SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user's password
    /// </summary>
    /// <param name="accessToken">User's current access token</param>
    /// <param name="newPassword">New password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if password was updated successfully</returns>
    Task<bool> UpdatePasswordAsync(string accessToken, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves user information from an access token
    /// </summary>
    /// <param name="accessToken">User's access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Supabase user object or null if token is invalid</returns>
    Task<Supabase.Gotrue.User?> GetUserAsync(string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies if an access token is valid
    /// </summary>
    /// <param name="accessToken">Access token to verify</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if token is valid</returns>
    Task<bool> VerifyTokenAsync(string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with new tokens</returns>
    Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
