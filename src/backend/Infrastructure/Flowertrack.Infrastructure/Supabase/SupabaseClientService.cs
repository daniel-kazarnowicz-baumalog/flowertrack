using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Supabase;
using Supabase.Gotrue.Interfaces;
using Supabase.Storage.Interfaces;
using SupabaseConfig = Flowertrack.Infrastructure.Configuration.SupabaseOptions;

namespace Flowertrack.Infrastructure.Supabase;

/// <summary>
/// Thread-safe implementation of Supabase client service
/// </summary>
public class SupabaseClientService : ISupabaseClient
{
    private readonly SupabaseConfig _options;
    private readonly ILogger<SupabaseClientService> _logger;
    private readonly Lazy<Client> _client;

    public SupabaseClientService(
        IOptions<SupabaseConfig> options,
        ILogger<SupabaseClientService> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);
        
        _options = options.Value ?? throw new ArgumentNullException(nameof(options), "Options value cannot be null");
        _logger = logger;

        // Lazy initialization for thread-safe singleton pattern
        _client = new Lazy<Client>(() => InitializeClient(), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <summary>
    /// Gets the Supabase client instance
    /// </summary>
    public Client GetClient() => _client.Value;

    /// <summary>
    /// Gets the Auth service
    /// </summary>
    public IGotrueClient<global::Supabase.Gotrue.User, global::Supabase.Gotrue.Session> Auth => _client.Value.Auth;

    /// <summary>
    /// Gets the Storage service
    /// </summary>
    public IStorageClient<global::Supabase.Storage.Bucket, global::Supabase.Storage.FileObject> Storage => _client.Value.Storage;

    /// <summary>
    /// Creates a new user in Supabase Auth
    /// </summary>
    public async Task<Guid> CreateUserAsync(
        string email,
        string? password = null,
        object? metadata = null,
        bool emailConfirm = false,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement Supabase Admin Auth user creation
        // This requires using Admin API with service key
        throw new NotImplementedException("Supabase user creation not yet implemented");
    }

    /// <summary>
    /// Gets a user by email address
    /// </summary>
    public async Task<global::Supabase.Gotrue.User?> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement user lookup by email using Admin API
        throw new NotImplementedException("Supabase user lookup not yet implemented");
    }

    /// <summary>
    /// Stores an activation token for a user
    /// </summary>
    public async Task StoreActivationTokenAsync(
        Guid userId,
        string token,
        DateTimeOffset expiry,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement token storage in Supabase database table
        throw new NotImplementedException("Token storage not yet implemented");
    }

    /// <summary>
    /// Sends an email using Supabase Edge Functions
    /// </summary>
    public async Task SendEmailAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement email sending via Supabase Edge Functions
        throw new NotImplementedException("Email sending not yet implemented");
    }

    private Client InitializeClient()
    {
        try
        {
            _logger.LogInformation("Initializing Supabase client for URL: {Url}", _options.Url);

            var supabaseOptions = new global::Supabase.SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = false, // Disable realtime for MVP
                // Use Service Key for server-side operations (bypasses RLS)
                Headers = new Dictionary<string, string>
                {
                    { "apikey", _options.ServiceKey }
                }
            };

            var client = new Client(_options.Url, _options.ServiceKey, supabaseOptions);

            _logger.LogInformation("Supabase client initialized successfully");

            return client;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Supabase client");
            throw;
        }
    }
}
