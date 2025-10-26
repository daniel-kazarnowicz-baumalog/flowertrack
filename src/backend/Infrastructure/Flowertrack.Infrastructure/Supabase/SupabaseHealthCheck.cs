using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Infrastructure.Supabase;

/// <summary>
/// Health check for Supabase connectivity (database and API)
/// </summary>
public class SupabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ISupabaseClient _supabaseClient;
    private readonly ILogger<SupabaseHealthCheck> _logger;

    public SupabaseHealthCheck(
        ApplicationDbContext dbContext,
        ISupabaseClient supabaseClient,
        ILogger<SupabaseHealthCheck> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _supabaseClient = supabaseClient ?? throw new ArgumentNullException(nameof(supabaseClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check database connectivity
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
            
            if (!canConnect)
            {
                _logger.LogWarning("Database connection check failed");
                return HealthCheckResult.Unhealthy("Cannot connect to PostgreSQL database");
            }

            // Check Supabase API availability by trying to get the client
            var client = _supabaseClient.GetClient();
            
            if (client == null)
            {
                _logger.LogWarning("Supabase client is not initialized");
                return HealthCheckResult.Degraded("Supabase client not properly initialized");
            }

            _logger.LogInformation("Supabase health check passed");
            
            return HealthCheckResult.Healthy("Supabase is operational", new Dictionary<string, object>
            {
                { "database", "connected" },
                { "supabase_api", "available" }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Supabase health check failed");
            
            return HealthCheckResult.Unhealthy(
                "Supabase health check failed", 
                ex,
                new Dictionary<string, object>
                {
                    { "error", ex.Message }
                });
        }
    }
}
