using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Supabase;
using Supabase.Gotrue.Interfaces;
using Supabase.Storage.Interfaces;
using System.Text.Json.Serialization;
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
        try
        {
            _logger.LogInformation("Creating Supabase user for email: {Email}", email);

            // Use Supabase Admin API via HTTP client
            // The C# SDK doesn't have full admin capabilities yet
            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10) // Set explicit timeout
            };
            httpClient.DefaultRequestHeaders.Add("apikey", _options.ServiceKey);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ServiceKey}");

            var requestBody = new
            {
                email,
                password = password ?? GenerateTemporaryPassword(),
                email_confirm = emailConfirm,
                user_metadata = metadata
            };

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody),
                System.Text.Encoding.UTF8,
                "application/json");

            _logger.LogDebug("Sending request to Supabase Admin API: {Url}", $"{_options.Url}/auth/v1/admin/users");

            var response = await httpClient.PostAsync(
                $"{_options.Url}/auth/v1/admin/users",
                content,
                cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Failed to create user in Supabase: {StatusCode} - {Error}",
                    response.StatusCode,
                    responseContent);
                throw new InvalidOperationException(
                    $"Failed to create user in Supabase: {response.StatusCode} - {responseContent}");
            }

            var userResponse = System.Text.Json.JsonSerializer.Deserialize<SupabaseUserResponse>(responseContent);

            if (userResponse == null || string.IsNullOrEmpty(userResponse.Id))
            {
                _logger.LogError("Invalid response from Supabase: {Response}", responseContent);
                throw new InvalidOperationException($"Failed to create user for email: {email}");
            }

            _logger.LogInformation("Successfully created Supabase user {UserId} for email: {Email}", 
                userResponse.Id, email);
            
            return Guid.Parse(userResponse.Id);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout creating Supabase user for email: {Email}", email);
            throw new InvalidOperationException($"Timeout creating user in Supabase for email: {email}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Supabase user for email: {Email}", email);
            throw;
        }
    }

    private static string GenerateTemporaryPassword()
    {
        // TODO: In production, this should generate a secure random password
        // For development, we use a simple default password: Password123!
        return "Password123!";
    }

    /// <summary>
    /// Gets a user by email address
    /// </summary>
    public async Task<global::Supabase.Gotrue.User?> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Looking up Supabase user by email: {Email}", email);

            // Use Supabase Admin API via HTTP client
            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10) // Set explicit timeout
            };
            httpClient.DefaultRequestHeaders.Add("apikey", _options.ServiceKey);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ServiceKey}");

            // Query users by email using Admin API
            // Note: Admin API doesn't support direct email filter, so we get by pagination
            // For MVP, we'll use a simpler approach - list all users and filter
            // In production, consider using database query instead
            _logger.LogDebug("Fetching users from Supabase Admin API");

            var response = await httpClient.GetAsync(
                $"{_options.Url}/auth/v1/admin/users",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Failed to lookup users: {StatusCode} - {Error}", 
                    response.StatusCode, error);
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("Received response from Supabase, parsing...");

            var usersResponse = System.Text.Json.JsonSerializer.Deserialize<SupabaseUsersListResponse>(responseBody);

            var userExists = usersResponse?.Users?.Any(u => 
                u.Email?.Equals(email, StringComparison.OrdinalIgnoreCase) == true) == true;

            if (userExists)
            {
                _logger.LogInformation("Found existing Supabase user for email: {Email}", email);
                
                // Return a dummy User object to indicate user exists
                // The actual User object is not needed, we only check for null
                return new global::Supabase.Gotrue.User();
            }
            
            _logger.LogInformation("No Supabase user found for email: {Email}", email);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout looking up Supabase user by email: {Email}", email);
            throw new InvalidOperationException($"Timeout looking up user in Supabase for email: {email}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to lookup Supabase user by email: {Email}", email);
            throw;
        }
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
        try
        {
            _logger.LogInformation("Storing activation token for user: {UserId}", userId);

            // For MVP, we'll log this instead of storing in database
            // In production, create an 'activation_tokens' table in Supabase
            _logger.LogWarning(
                "Token storage not fully implemented. Token for user {UserId} expires at {Expiry}",
                userId, expiry);

            // TODO: Implement token storage in Supabase table
            // Example implementation:
            // var httpClient = new HttpClient();
            // httpClient.DefaultRequestHeaders.Add("apikey", _options.ServiceKey);
            // httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ServiceKey}");
            // var data = new { user_id = userId, token, expires_at = expiry };
            // await httpClient.PostAsJsonAsync($"{_options.Url}/rest/v1/activation_tokens", data);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store activation token for user: {UserId}", userId);
            throw;
        }
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
        try
        {
            _logger.LogInformation("Sending email to: {To}, subject: {Subject}", to, subject);

            // For MVP, we'll log the email instead of sending it
            // In production, you would call a Supabase Edge Function or use an email service
            _logger.LogWarning(
                "Email sending not fully implemented. Email would be sent to {To} with subject '{Subject}'.\nBody: {Body}",
                to, subject, htmlBody);

            // TODO: Implement actual email sending via:
            // 1. Supabase Edge Function (recommended)
            // 2. Third-party service like SendGrid, AWS SES, etc.
            
            // Example Edge Function call (when implemented):
            // var client = _client.Value;
            // var payload = new { to, subject, html_body = htmlBody };
            // await client.Functions.Invoke("send-email", payload);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to: {To}", to);
            throw;
        }
    }

    /// <summary>
    /// Signs in a user using direct HTTP call to Supabase Auth API
    /// This bypasses the supabase-csharp library which has issues with API key headers
    /// </summary>
    public async Task<SignInHttpResult> SignInWithHttpAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Attempting direct HTTP sign-in for user: {Email}", email);

            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            // Set required headers for Supabase Auth API
            httpClient.DefaultRequestHeaders.Add("apikey", _options.AnonKey);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.AnonKey}");

            var requestBody = new
            {
                email,
                password
            };

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody),
                System.Text.Encoding.UTF8,
                "application/json");

            var authUrl = $"{_options.Url}/auth/v1/token?grant_type=password";
            _logger.LogDebug("Sending sign-in request to: {Url}", authUrl);

            var response = await httpClient.PostAsync(authUrl, content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Sign-in failed: {StatusCode} - {Error}",
                    response.StatusCode,
                    responseContent);

                // Try to parse error message
                try
                {
                    var errorDoc = System.Text.Json.JsonDocument.Parse(responseContent);
                    var errorMessage = errorDoc.RootElement.TryGetProperty("error_description", out var desc)
                        ? desc.GetString()
                        : errorDoc.RootElement.TryGetProperty("message", out var msg)
                            ? msg.GetString()
                            : "Authentication failed";
                    return SignInHttpResult.CreateFailure(errorMessage ?? "Authentication failed");
                }
                catch
                {
                    return SignInHttpResult.CreateFailure($"Authentication failed: {response.StatusCode}");
                }
            }

            // Parse successful response
            var jsonDoc = System.Text.Json.JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;

            var accessToken = root.GetProperty("access_token").GetString();
            var refreshToken = root.GetProperty("refresh_token").GetString();
            var expiresIn = root.GetProperty("expires_in").GetInt64();
            
            string? userId = null;
            string? userEmail = null;
            
            if (root.TryGetProperty("user", out var userElement))
            {
                userId = userElement.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                userEmail = userElement.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
            }

            _logger.LogInformation("Successfully signed in user {Email} via direct HTTP", email);

            return SignInHttpResult.CreateSuccess(
                accessToken ?? string.Empty,
                refreshToken ?? string.Empty,
                expiresIn,
                userId ?? string.Empty,
                userEmail ?? email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Direct HTTP sign-in failed for user {Email}", email);
            return SignInHttpResult.CreateFailure($"Sign-in failed: {ex.Message}");
        }
    }

    private Client InitializeClient()
    {
        try
        {
            _logger.LogInformation("Initializing Supabase client for URL: {Url}", _options.Url);
            
            // Debug: Log key prefixes for troubleshooting
            var anonKeyPrefix = !string.IsNullOrEmpty(_options.AnonKey) && _options.AnonKey.Length > 20 
                ? _options.AnonKey[..20] + "..." 
                : "EMPTY_OR_SHORT";
            _logger.LogInformation("Using AnonKey starting with: {AnonKeyPrefix}", anonKeyPrefix);

            var supabaseOptions = new global::Supabase.SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = false, // Disable realtime for MVP
                // Headers for additional API operations - ServiceKey used for admin operations
                Headers = new Dictionary<string, string>
                {
                    { "apikey", _options.AnonKey }
                }
            };

            // Use AnonKey for authentication operations (SignIn, SignUp, etc.)
            // ServiceKey is only used for Admin API calls via direct HTTP client
            var client = new Client(_options.Url, _options.AnonKey, supabaseOptions);

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

/// <summary>
/// Response from Supabase Admin API for user creation
/// </summary>
internal class SupabaseUserResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

/// <summary>
/// Response from Supabase Admin API for listing users
/// </summary>
internal class SupabaseUsersListResponse
{
    [JsonPropertyName("users")]
    public List<SupabaseUserResponse>? Users { get; set; }
}
