using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Entities.Users;
using Flowertrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Infrastructure.Data;

/// <summary>
/// Seeds the database with the initial Service Administrator user.
/// This seeder runs automatically on first application start when the database is empty.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Service Admin credentials for development.
    /// Email: admin@flowertrack.dev
    /// Password: Admin123!
    /// </summary>
    private const string AdminEmail = "admin@flowertrack.dev";
    private const string AdminPassword = "Admin123!";
    private const string AdminFirstName = "System";
    private const string AdminLastName = "Administrator";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // Step 1: Ensure Roles exist (from EF Core HasData configuration)
            if (!await context.Roles.AnyAsync())
            {
                logger.LogInformation("Seeding roles...");
                await context.Roles.AddRangeAsync(Role.GetAllRoles());
                await context.SaveChangesAsync();
                logger.LogInformation("Roles seeded successfully.");
            }

            // Step 2: Check if Service Admin already exists
            var existingAdmin = await context.ServiceUsers
                .FirstOrDefaultAsync(su => su.Email.Value == AdminEmail);

            if (existingAdmin != null)
            {
                logger.LogInformation("Service Admin already exists. Skipping seed.");
                return;
            }

            logger.LogInformation("Creating Service Admin user...");

            // Step 3: Create user in Supabase Auth (auth.users table)
            // Generate a fixed UUID for the auth user so we can link it
            var authUserId = Guid.NewGuid();

            // Build JSON strings separately to avoid escaping issues
            var appMetaData = @"{""provider"":""email"",""providers"":[""email""],""role"":""ServiceAdministrator""}";
            var userMetaData = $@"{{""first_name"":""{AdminFirstName}"",""last_name"":""{AdminLastName}""}}";

            // Use parameterized query to avoid SQL injection and format string issues
            var sql = @"
INSERT INTO auth.users (
    instance_id,
    id,
    aud,
    role,
    email,
    encrypted_password,
    email_confirmed_at,
    created_at,
    updated_at,
    raw_app_meta_data,
    raw_user_meta_data,
    confirmation_token,
    email_change,
    email_change_token_new,
    recovery_token
) VALUES (
    '00000000-0000-0000-0000-000000000000'::uuid,
    @p0::uuid,
    'authenticated',
    'authenticated',
    @p1,
    crypt(@p2, gen_salt('bf')),
    NOW(),
    NOW(),
    NOW(),
    @p3::jsonb,
    @p4::jsonb,
    '',
    '',
    '',
    ''
)
ON CONFLICT (id) DO NOTHING;";

            await context.Database.ExecuteSqlRawAsync(sql,
                authUserId.ToString(),
                AdminEmail,
                AdminPassword,
                appMetaData,
                userMetaData);

            logger.LogInformation("Created user in Supabase Auth: {AuthUserId}", authUserId);

            // Step 4: Create ServiceUser in application database
            var serviceUserId = Guid.NewGuid();
            var serviceUser = ServiceUser.Create(
                userId: serviceUserId,
                firstName: AdminFirstName,
                lastName: AdminLastName,
                email: AdminEmail,
                phoneNumber: null,
                specialization: "System Administration");

            // Link to Supabase Auth user
            serviceUser.LinkToSupabaseUser(authUserId);

            // Set password hash for local authentication (BCrypt.Net compatible)
            var passwordHash = passwordHasher.HashPassword(AdminPassword);
            serviceUser.SetPasswordHash(passwordHash);

            // Activate the user
            serviceUser.Activate();

            // Assign ServiceAdministrator role (id = 1)
            serviceUser.AssignRole(Role.ServiceAdministrator.Id);

            await context.ServiceUsers.AddAsync(serviceUser);
            await context.SaveChangesAsync();

            logger.LogInformation("Service Admin created successfully!");
            logger.LogInformation("  Email: {Email}", AdminEmail);
            logger.LogInformation("  Password: {Password}", AdminPassword);
            logger.LogInformation("  ServiceUser ID: {ServiceUserId}", serviceUserId);
            logger.LogInformation("  Supabase Auth ID: {AuthUserId}", authUserId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
