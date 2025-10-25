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
}
