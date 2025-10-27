using System.Text;
using Flowertrack.Application;
using Flowertrack.Infrastructure;
using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Infrastructure.Configuration;
using Flowertrack.Infrastructure.Data;
using Flowertrack.Infrastructure.Persistence;
using Flowertrack.Infrastructure.Supabase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Flowertrack.Api.Middleware;
using Flowertrack.Api.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        "logs/flowertrack-.log",
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 104857600, // 100MB
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting FLOWerTRACK API");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId());

    // Add services to the container.
    builder.Services.AddControllers();

    // Add Application and Infrastructure layers
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // Configure Supabase Options
    builder.Services.Configure<SupabaseOptions>(
        builder.Configuration.GetSection("Supabase"));

    // Configure Database Options
    builder.Services.Configure<DatabaseOptions>(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=flowertrack_dev;Username=postgres;Password=postgres";

        var dbSection = builder.Configuration.GetSection("Database");
        options.CommandTimeout = dbSection.GetValue<int>("CommandTimeout", 30);
        options.EnableSensitiveDataLogging = dbSection.GetValue<bool>("EnableSensitiveDataLogging", false);
    });

    // Validate configuration options on startup
    builder.Services.AddOptions<SupabaseOptions>()
        .Bind(builder.Configuration.GetSection("Supabase"))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    builder.Services.AddOptions<DatabaseOptions>()
        .Configure(options =>
        {
            options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Database=flowertrack_dev;Username=postgres;Password=postgres";

            var dbSection = builder.Configuration.GetSection("Database");
            options.CommandTimeout = dbSection.GetValue<int>("CommandTimeout", 30);
            options.EnableSensitiveDataLogging = dbSection.GetValue<bool>("EnableSensitiveDataLogging", false);
        })
        .ValidateDataAnnotations()
        .ValidateOnStart();

    // Register Supabase services
    builder.Services.AddSingleton<ISupabaseClient, SupabaseClientService>();
    builder.Services.AddScoped<IFileStorageService, SupabaseStorageService>();

    // Add Database Context
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Host=localhost;Database=flowertrack_dev;Username=postgres;Password=postgres";

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));

    // Add Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddOpenApi();

    // Configure JWT Authentication
    var supabaseUrl = builder.Configuration["Supabase:Url"] ?? "";
    var jwtSecret = builder.Configuration["Supabase:JwtSecret"] ?? "";

    if (!string.IsNullOrEmpty(jwtSecret))
    {
        var jwtSecretBytes = Encoding.UTF8.GetBytes(jwtSecret);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = supabaseUrl,
                    ValidAudience = supabaseUrl,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtSecretBytes),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("Authentication failed: {Error}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogInformation("Token validated for user: {User}", context.Principal?.Identity?.Name ?? "Unknown");
                        return Task.CompletedTask;
                    }
                };
            });
    }

    // Configure CORS
    var frontendUrl = builder.Configuration["Frontend:Url"] ?? "http://localhost:5173";
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(frontendUrl, "http://localhost:5173", "http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // Add Health Checks
    builder.Services.AddHealthChecks()
        .AddNpgSql(connectionString, name: "database", timeout: TimeSpan.FromSeconds(3), tags: new[] { "db", "postgresql" })
        .AddCheck<SupabaseHealthCheck>("supabase", tags: new[] { "supabase", "database", "api" });

    var app = builder.Build();

    // Configure the HTTP request pipeline

    // Add Global Exception Handler (must be first)
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    // Add Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            var requestHost = httpContext.Request.Host.Value ?? "Unknown";
            diagnosticContext.Set("RequestHost", requestHost);
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            diagnosticContext.Set("UserAgent", string.IsNullOrEmpty(userAgent) ? "Unknown" : userAgent);
        };
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "FLOWerTRACK API v1");
        });
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseCors();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Map Health Check endpoints
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckResponseWriter.WriteResponse
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = HealthCheckResponseWriter.WriteResponse
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false, // Just check if service is running
        ResponseWriter = HealthCheckResponseWriter.WriteResponse
    });

    app.MapHealthChecks("/health/supabase", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("supabase"),
        ResponseWriter = HealthCheckResponseWriter.WriteResponse
    });

    // Test endpoints for exception handling (Development only)
    if (app.Environment.IsDevelopment())
    {
        app.MapGet("/test/not-found", () =>
        {
            throw new Flowertrack.Api.Exceptions.NotFoundException("TestResource", 123);
        });

        app.MapGet("/test/validation", () =>
        {
            var errors = new Dictionary<string, string[]>
            {
                { "Name", new[] { "Name is required", "Name must be at least 3 characters" } },
                { "Email", new[] { "Invalid email format" } }
            };
            throw new Flowertrack.Api.Exceptions.ValidationException(errors);
        });

        app.MapGet("/test/unauthorized", () =>
        {
            throw new Flowertrack.Api.Exceptions.UnauthorizedException();
        });

        app.MapGet("/test/forbidden", () =>
        {
            throw new Flowertrack.Api.Exceptions.ForbiddenException("You don't have access to this resource");
        });

        app.MapGet("/test/domain", () =>
        {
            throw new Flowertrack.Api.Exceptions.DomainException("Cannot close ticket because it has pending tasks");
        });

        app.MapGet("/test/server-error", () =>
        {
            throw new InvalidOperationException("Simulated server error for testing exception handling");
        });
    }

    // Seed database in development environment
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Applying database migrations...");
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            logger.LogInformation("Seeding database...");
            await DatabaseSeeder.SeedAsync(services);
            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
