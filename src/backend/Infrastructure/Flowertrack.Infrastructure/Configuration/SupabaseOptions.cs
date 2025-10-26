using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Infrastructure.Configuration;

/// <summary>
/// Configuration options for Supabase integration
/// </summary>
public class SupabaseOptions
{
    /// <summary>
    /// The Supabase project URL
    /// </summary>
    [Required(ErrorMessage = "Supabase URL is required")]
    [Url(ErrorMessage = "Supabase URL must be a valid URL")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The anonymous public key for client-side operations
    /// </summary>
    [Required(ErrorMessage = "Supabase Anon Key is required")]
    public string AnonKey { get; set; } = string.Empty;

    /// <summary>
    /// The service role key for server-side operations (bypass RLS)
    /// </summary>
    [Required(ErrorMessage = "Supabase Service Key is required")]
    public string ServiceKey { get; set; } = string.Empty;

    /// <summary>
    /// JWT secret used for token validation
    /// </summary>
    [Required(ErrorMessage = "Supabase JWT Secret is required")]
    public string JwtSecret { get; set; } = string.Empty;
}
