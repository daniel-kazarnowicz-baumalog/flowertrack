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

    // Add services to the container
    builder.Services.AddOpenApi();

    // Configure CORS
    var frontendUrl = builder.Configuration["Frontend:Url"] ?? "http://localhost:5173";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins(frontendUrl)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // Configure Health Checks
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddHealthChecks()
        .AddNpgSql(
            connectionString ?? "Host=localhost;Database=flowertrack;Username=postgres;Password=postgres",
            name: "database",
            timeout: TimeSpan.FromSeconds(3),
            tags: new[] { "db", "postgresql" });

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
        app.MapOpenApi();
    }

    // Enable CORS
    app.UseCors("AllowFrontend");

    app.UseHttpsRedirection();

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

    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

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

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
