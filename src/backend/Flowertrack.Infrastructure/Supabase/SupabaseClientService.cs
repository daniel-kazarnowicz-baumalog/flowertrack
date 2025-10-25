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
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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
