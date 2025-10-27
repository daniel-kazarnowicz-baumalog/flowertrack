using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Domain.Repositories;
using Flowertrack.Infrastructure.Persistence;
using Flowertrack.Infrastructure.Persistence.Repositories;
using Flowertrack.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flowertrack.Infrastructure;

/// <summary>
/// Dependency injection registration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database Context
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            });

            // Enable sensitive data logging in development
            if (configuration.GetValue<bool>("Logging:EnableSensitiveDataLogging"))
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // DbContext interfaces
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IMachineRepository, MachineRepository>();
        services.AddScoped<IServiceUserRepository, ServiceUserRepository>();
        services.AddScoped<IOrganizationUserRepository, OrganizationUserRepository>();

        // Infrastructure Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITokenGenerator, TokenGeneratorService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Supabase Client (already registered in Api layer, but we expose the interface)
        // services.AddScoped<ISupabaseClient, SupabaseClientService>();

        // HTTP Context Accessor for CurrentUserService
        services.AddHttpContextAccessor();

        return services;
    }
}
