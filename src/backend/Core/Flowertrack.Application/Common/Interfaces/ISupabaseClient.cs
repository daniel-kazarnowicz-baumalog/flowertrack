using Supabase;
using Supabase.Gotrue.Interfaces;
using Supabase.Storage.Interfaces;

namespace Flowertrack.Application.Common.Interfaces;

/// <summary>
/// Interface for Supabase client service
/// </summary>
public interface ISupabaseClient
{
    /// <summary>
    /// Gets the Supabase client instance
    /// </summary>
    Client GetClient();

    /// <summary>
    /// Gets the Auth service
    /// </summary>
    IGotrueClient<global::Supabase.Gotrue.User, global::Supabase.Gotrue.Session> Auth { get; }

    /// <summary>
    /// Gets the Storage service
    /// </summary>
    IStorageClient<global::Supabase.Storage.Bucket, global::Supabase.Storage.FileObject> Storage { get; }

    /// <summary>
    /// Creates a new user in Supabase Auth
    /// </summary>
    Task<Guid> CreateUserAsync(
        string email,
        string? password = null,
        object? metadata = null,
        bool emailConfirm = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by email address
    /// </summary>
    Task<global::Supabase.Gotrue.User?> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores an activation token for a user
    /// </summary>
    Task StoreActivationTokenAsync(
        Guid userId,
        string token,
        DateTimeOffset expiry,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email using Supabase Edge Functions
    /// </summary>
    Task SendEmailAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}
