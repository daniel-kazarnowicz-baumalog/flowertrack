using System.Text;
using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Infrastructure.Configuration;
using Flowertrack.Infrastructure.Data;
using Flowertrack.Infrastructure.Persistence;
using Flowertrack.Infrastructure.Supabase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/flowertrack-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

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

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "database")
    .AddCheck<SupabaseHealthCheck>("supabase", tags: new[] { "supabase", "database", "api" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FLOWerTRACK API v1");
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/supabase", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("supabase")
});

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
