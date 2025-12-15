using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Microsoft.Extensions.Logging;
using Supabase.Gotrue;

namespace Flowertrack.Infrastructure.Services.Authentication;

/// <summary>
/// Service for managing authentication through Supabase Auth
/// Wraps the Supabase Auth client with error handling and logging
/// </summary>
public class SupabaseAuthService : IAuthService
{
    private readonly ISupabaseClient _supabaseClient;
    private readonly ILogger<SupabaseAuthService> _logger;

    public SupabaseAuthService(
        ISupabaseClient supabaseClient,
        ILogger<SupabaseAuthService> logger)
    {
        _supabaseClient = supabaseClient ?? throw new ArgumentNullException(nameof(supabaseClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<AuthResult> SignUpAsync(
        string email, 
        string password, 
        UserMetadata metadata, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Attempting to sign up user with email: {Email}", email);

            var options = new SignUpOptions
            {
                Data = metadata.ToDictionary()
            };

            var session = await _supabaseClient.Auth.SignUp(email, password, options);

            if (session?.User == null)
            {
                _logger.LogWarning("Sign up failed for user {Email}: No session or user returned", email);
                return AuthResult.CreateFailure("Sign up failed. Please try again.");
            }

            _logger.LogInformation("Successfully signed up user {Email} with ID {UserId}", 
                email, session.User.Id);

            var expiresAt = session.ExpiresIn > 0
                ? DateTimeOffset.UtcNow.AddSeconds(session.ExpiresIn)
                : DateTimeOffset.UtcNow.AddHours(1);

            return AuthResult.CreateSuccess(
                session.AccessToken ?? string.Empty,
                session.RefreshToken ?? string.Empty,
                session.User,
                expiresAt
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sign up user {Email}", email);
            return AuthResult.CreateFailure($"Sign up failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<AuthResult> SignInAsync(
        string email, 
        string password, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Attempting to sign in user with email: {Email}", email);

            // Use direct HTTP call instead of supabase-csharp library
            // The library has issues with API key headers in some environments
            var result = await _supabaseClient.SignInWithHttpAsync(email, password, cancellationToken);

            if (!result.Success)
            {
                _logger.LogWarning("Sign in failed for user {Email}: {Error}", email, result.ErrorMessage);
                return AuthResult.CreateFailure(result.ErrorMessage ?? "Invalid email or password.");
            }

            _logger.LogInformation("Successfully signed in user {Email} with ID {UserId}", 
                email, result.UserId);

            var expiresAt = result.ExpiresIn > 0
                ? DateTimeOffset.UtcNow.AddSeconds(result.ExpiresIn)
                : DateTimeOffset.UtcNow.AddHours(1);

            // Create a minimal User object for backward compatibility
            var user = new global::Supabase.Gotrue.User
            {
                Id = result.UserId,
                Email = result.Email
            };

            return AuthResult.CreateSuccess(
                result.AccessToken ?? string.Empty,
                result.RefreshToken ?? string.Empty,
                user,
                expiresAt
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sign in user {Email}", email);
            return AuthResult.CreateFailure($"Sign in failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<bool> SignOutAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Attempting to sign out user");

            await _supabaseClient.Auth.SignOut();

            _logger.LogInformation("Successfully signed out user");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sign out user");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> SendPasswordResetEmailAsync(
        string email, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sending password reset email to: {Email}", email);

            await _supabaseClient.Auth.ResetPasswordForEmail(email);

            _logger.LogInformation("Successfully sent password reset email to {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdatePasswordAsync(
        string accessToken, 
        string newPassword, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Attempting to update password for user");

            var attributes = new UserAttributes
            {
                Password = newPassword
            };

            var result = await _supabaseClient.Auth.Update(attributes);

            if (result == null)
            {
                _logger.LogWarning("Password update failed: No user returned");
                return false;
            }

            _logger.LogInformation("Successfully updated password for user {UserId}", result.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update password");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<User?> GetUserAsync(
        string accessToken, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Retrieving user from access token");

            var user = await _supabaseClient.Auth.GetUser(accessToken);

            if (user == null)
            {
                _logger.LogWarning("No user found for provided access token");
                return null;
            }

            _logger.LogDebug("Successfully retrieved user {UserId}", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve user from access token");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> VerifyTokenAsync(
        string accessToken, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await GetUserAsync(accessToken, cancellationToken);
            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify token");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<AuthResult> RefreshTokenAsync(
        string refreshToken, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Attempting to refresh access token");

            var session = await _supabaseClient.Auth.RefreshSession();

            if (session?.User == null)
            {
                _logger.LogWarning("Token refresh failed: No session returned");
                return AuthResult.CreateFailure("Failed to refresh token. Please sign in again.");
            }

            _logger.LogInformation("Successfully refreshed token for user {UserId}", session.User.Id);

            var expiresAt = session.ExpiresIn > 0
                ? DateTimeOffset.UtcNow.AddSeconds(session.ExpiresIn)
                : DateTimeOffset.UtcNow.AddHours(1);

            return AuthResult.CreateSuccess(
                session.AccessToken ?? string.Empty,
                session.RefreshToken ?? string.Empty,
                session.User,
                expiresAt
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh token");
            return AuthResult.CreateFailure($"Token refresh failed: {ex.Message}");
        }
    }
}
