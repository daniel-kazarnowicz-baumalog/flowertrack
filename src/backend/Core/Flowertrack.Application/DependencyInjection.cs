using System.Reflection;
using Flowertrack.Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Flowertrack.Application;

/// <summary>
/// Dependency injection registration for Application layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR with all handlers from this assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // Pipeline behaviors - ORDER MATTERS!
            // 1. Logging - first to log everything
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));

            // 2. Validation - validate before processing
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));

            // 3. UnitOfWork - commit after successful processing
            cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });

        // FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
