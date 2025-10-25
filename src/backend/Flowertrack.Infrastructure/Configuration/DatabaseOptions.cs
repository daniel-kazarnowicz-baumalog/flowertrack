using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Infrastructure.Configuration;

/// <summary>
/// Configuration options for database connection
/// </summary>
public class DatabaseOptions
{
    /// <summary>
    /// PostgreSQL connection string
    /// </summary>
    [Required(ErrorMessage = "Database connection string is required")]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Command timeout in seconds (default: 30)
    /// </summary>
    [Range(1, 300, ErrorMessage = "Command timeout must be between 1 and 300 seconds")]
    public int CommandTimeout { get; set; } = 30;

    /// <summary>
    /// Enable sensitive data logging (only for development)
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;
}
