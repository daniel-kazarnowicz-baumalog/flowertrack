using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Entities.Users;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.ValueObjects;
using Flowertrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Infrastructure.Data;

/// <summary>
/// Seeds the database with sample data for development and testing
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Ensure Roles exist
            if (!await context.Roles.AnyAsync())
            {
                logger.LogInformation("Seeding roles...");
                await context.Roles.AddRangeAsync(Role.GetAllRoles());
                await context.SaveChangesAsync();
            }

            // Check if data already exists
            if (await context.Organizations.AnyAsync())
            {
                logger.LogInformation("Database already contains data. Skipping organization seed.");
                
                // Even if organizations exist, seed test users for development
                await SeedTestUsersAsync(context, logger);
                return;
            }

            logger.LogInformation("Seeding database with sample data...");

            // Create sample organizations
            var organizations = new List<Organization>
            {
                CreateOrganization(
                    "Baumalog Sp. z o.o.",
                    "kontakt@baumalog.pl",
                    "+48 123 456 789",
                    "ul. Przemysłowa 15",
                    "Warszawa",
                    "00-001",
                    "Polska"
                ),
                CreateOrganization(
                    "TechCorp Industries",
                    "info@techcorp.com",
                    "+48 987 654 321",
                    "ul. Innowacyjna 42",
                    "Kraków",
                    "30-001",
                    "Polska"
                ),
                CreateOrganization(
                    "Global Manufacturing Ltd",
                    "contact@globalmanuf.com",
                    "+48 555 123 456",
                    "ul. Fabryczna 7",
                    "Wrocław",
                    "50-001",
                    "Polska"
                ),
                CreateOrganization(
                    "Smart Factory Solutions",
                    "hello@smartfactory.pl",
                    "+48 111 222 333",
                    "ul. Nowoczesna 99",
                    "Poznań",
                    "60-001",
                    "Polska"
                ),
                CreateOrganization(
                    "Industrial Automation Co",
                    "sales@indauto.com",
                    "+48 444 555 666",
                    "ul. Automatyczna 24",
                    "Gdańsk",
                    "80-001",
                    "Polska"
                )
            };

            // Generate API keys for organizations
            foreach (var org in organizations)
            {
                org.GenerateApiKey();
            }

            // Set some organizations to different statuses
            organizations[1].UpdateServiceStatus(ServiceStatus.Suspended, "Manual suspension for testing");
            organizations[3].UpdateServiceStatus(ServiceStatus.Active, "Reactivation after suspension");

            // Note: Notes are set during organization creation

            // Add organizations to context
            await context.Organizations.AddRangeAsync(organizations);
            await context.SaveChangesAsync();

            logger.LogInformation($"Successfully seeded {organizations.Count} organizations.");
            
            // Seed test users for development
            await SeedTestUsersAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    /// <summary>
    /// Seeds test users for development environment
    /// </summary>
    private static async Task SeedTestUsersAsync(ApplicationDbContext context, ILogger logger)
    {
        try
        {
            logger.LogInformation("Seeding test users for development...");

            // Check if test users already exist in auth.users
            var existingAuthUsersQuery = @"
                SELECT COUNT(*) 
                FROM auth.users 
                WHERE email IN ('admin@flowertrack.dev', 'tech@flowertrack.dev', 'client@test.com', 'operator@test.com')";
            
            var existingCount = await context.Database.ExecuteSqlRawAsync(existingAuthUsersQuery);
            
            if (existingCount >= 4)
            {
                logger.LogInformation("Test users already exist in Supabase Auth. Skipping auth user creation.");
            }
            else
            {
                // Create test users in Supabase Auth (auth.users table)
                logger.LogInformation("Creating test users in Supabase Auth...");
                
                // Use DO block to check existence before inserting
                await context.Database.ExecuteSqlRawAsync(@"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM auth.users WHERE email = 'admin@flowertrack.dev') THEN
        INSERT INTO auth.users (
            instance_id, id, aud, role, email, encrypted_password,
            email_confirmed_at, created_at, updated_at,
            raw_app_meta_data, raw_user_meta_data,
            confirmation_token, email_change, email_change_token_new, recovery_token
        ) VALUES (
            '00000000-0000-0000-0000-000000000000', gen_random_uuid(),
            'authenticated', 'authenticated', 'admin@flowertrack.dev',
            crypt('Admin123!', gen_salt('bf')), NOW(), NOW(), NOW(),
            '{{""provider"":""email"",""providers"":[""email""]}}', '{{}}',
            '', '', '', ''
        );
    END IF;
END $$");

                await context.Database.ExecuteSqlRawAsync(@"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM auth.users WHERE email = 'tech@flowertrack.dev') THEN
        INSERT INTO auth.users (
            instance_id, id, aud, role, email, encrypted_password,
            email_confirmed_at, created_at, updated_at,
            raw_app_meta_data, raw_user_meta_data,
            confirmation_token, email_change, email_change_token_new, recovery_token
        ) VALUES (
            '00000000-0000-0000-0000-000000000000', gen_random_uuid(),
            'authenticated', 'authenticated', 'tech@flowertrack.dev',
            crypt('Tech123!', gen_salt('bf')), NOW(), NOW(), NOW(),
            '{{""provider"":""email"",""providers"":[""email""]}}', '{{}}',
            '', '', '', ''
        );
    END IF;
END $$");

                await context.Database.ExecuteSqlRawAsync(@"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM auth.users WHERE email = 'client@test.com') THEN
        INSERT INTO auth.users (
            instance_id, id, aud, role, email, encrypted_password,
            email_confirmed_at, created_at, updated_at,
            raw_app_meta_data, raw_user_meta_data,
            confirmation_token, email_change, email_change_token_new, recovery_token
        ) VALUES (
            '00000000-0000-0000-0000-000000000000', gen_random_uuid(),
            'authenticated', 'authenticated', 'client@test.com',
            crypt('Client123!', gen_salt('bf')), NOW(), NOW(), NOW(),
            '{{""provider"":""email"",""providers"":[""email""]}}', '{{}}',
            '', '', '', ''
        );
    END IF;
END $$");

                await context.Database.ExecuteSqlRawAsync(@"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM auth.users WHERE email = 'operator@test.com') THEN
        INSERT INTO auth.users (
            instance_id, id, aud, role, email, encrypted_password,
            email_confirmed_at, created_at, updated_at,
            raw_app_meta_data, raw_user_meta_data,
            confirmation_token, email_change, email_change_token_new, recovery_token
        ) VALUES (
            '00000000-0000-0000-0000-000000000000', gen_random_uuid(),
            'authenticated', 'authenticated', 'operator@test.com',
            crypt('Operator123!', gen_salt('bf')), NOW(), NOW(), NOW(),
            '{{""provider"":""email"",""providers"":[""email""]}}', '{{}}',
            '', '', '', ''
        );
    END IF;
END $$");
                
                logger.LogInformation("Test users created in Supabase Auth.");
            }

            // Get Supabase User IDs
            var getUserIdsSql = @"
                SELECT id, email FROM auth.users 
                WHERE email IN ('admin@flowertrack.dev', 'tech@flowertrack.dev', 'client@test.com', 'operator@test.com')";

            var authUsers = await context.Database
                .SqlQueryRaw<AuthUserDto>(getUserIdsSql)
                .ToListAsync();

            var adminAuthUser = authUsers.FirstOrDefault(u => u.Email == "admin@flowertrack.dev");
            var techAuthUser = authUsers.FirstOrDefault(u => u.Email == "tech@flowertrack.dev");
            var clientAuthUser = authUsers.FirstOrDefault(u => u.Email == "client@test.com");
            var operatorAuthUser = authUsers.FirstOrDefault(u => u.Email == "operator@test.com");

            // Ensure Test Organization exists
            var testOrgId = Guid.Parse("550e8400-e29b-41d4-a716-446655440001");
            var testOrg = await context.Organizations.FindAsync(testOrgId);
            
            if (testOrg == null)
            {
                logger.LogInformation("Creating Test Organization...");
                testOrg = Organization.Create(
                    name: "Test Organization",
                    email: "contact@test.com",
                    phone: "+48 123 456 789",
                    address: null,
                    city: null,
                    postalCode: null,
                    country: null);
                
                // Set the ID manually (for testing purposes)
                typeof(Organization).GetProperty("Id")!.SetValue(testOrg, testOrgId);
                
                // Set audit fields manually since we're using reflection
                typeof(Organization).GetProperty("CreatedAt")!.SetValue(testOrg, DateTimeOffset.UtcNow);
                typeof(Organization).GetProperty("UpdatedAt")!.SetValue(testOrg, DateTimeOffset.UtcNow);
                
                // Set contract dates (1 year contract)
                testOrg.RenewContract(DateTimeOffset.UtcNow.AddYears(1));
                
                await context.Organizations.AddAsync(testOrg);
                await context.SaveChangesAsync();
                logger.LogInformation("Test Organization created.");
            }

            // Create ServiceUsers linked to Supabase Auth
            if (adminAuthUser != null)
            {
                var existingAdmin = await context.ServiceUsers
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(su => su.SupabaseUserId == adminAuthUser.Id);
                
                if (existingAdmin == null)
                {
                    var adminUser = ServiceUser.Create(
                        userId: Guid.NewGuid(),
                        firstName: "Admin",
                        lastName: "User",
                        email: "admin@flowertrack.dev",
                        phoneNumber: "+48 123 456 001",
                        specialization: "Administrator");
                    
                    // Link to Supabase Auth user
                    typeof(ServiceUser).GetProperty("SupabaseUserId")!.SetValue(adminUser, adminAuthUser.Id);
                    adminUser.Activate();
                    adminUser.SetAvailability(true);
                    adminUser.AssignRole(Role.ServiceAdministrator.Id);
                    
                    await context.ServiceUsers.AddAsync(adminUser);
                    logger.LogInformation("Admin ServiceUser created and linked.");
                }
                else if (!existingAdmin.HasRole(Role.ServiceAdministrator.Id))
                {
                    existingAdmin.AssignRole(Role.ServiceAdministrator.Id);
                    logger.LogInformation("Assigned ServiceAdministrator role to existing Admin user.");
                }
            }

            if (techAuthUser != null)
            {
                var existingTech = await context.ServiceUsers
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(su => su.SupabaseUserId == techAuthUser.Id);
                
                if (existingTech == null)
                {
                    var techUser = ServiceUser.Create(
                        userId: Guid.NewGuid(),
                        firstName: "Tech",
                        lastName: "Support",
                        email: "tech@flowertrack.dev",
                        phoneNumber: "+48 123 456 002",
                        specialization: "Technical Support");
                    
                    typeof(ServiceUser).GetProperty("SupabaseUserId")!.SetValue(techUser, techAuthUser.Id);
                    techUser.Activate();
                    techUser.SetAvailability(true);
                    techUser.AssignRole(Role.ServiceTechnician.Id);
                    
                    await context.ServiceUsers.AddAsync(techUser);
                    logger.LogInformation("Tech ServiceUser created and linked.");
                }
                else if (!existingTech.HasRole(Role.ServiceTechnician.Id))
                {
                    existingTech.AssignRole(Role.ServiceTechnician.Id);
                    logger.LogInformation("Assigned ServiceTechnician role to existing Tech user.");
                }
            }

            // Create OrganizationUsers linked to Supabase Auth
            if (clientAuthUser != null)
            {
                var existingClient = await context.OrganizationUsers
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(ou => ou.SupabaseUserId == clientAuthUser.Id);
                
                if (existingClient == null)
                {
                    var clientUser = OrganizationUser.Create(
                        userId: Guid.NewGuid(),
                        firstName: "Client",
                        lastName: "Admin",
                        email: "client@test.com",
                        organizationId: testOrgId,
                        phoneNumber: "+48 123 456 003");
                    
                    typeof(OrganizationUser).GetProperty("SupabaseUserId")!.SetValue(clientUser, clientAuthUser.Id);
                    clientUser.Activate();
                    clientUser.AssignRole(Role.OrganizationAdministrator.Id);
                    
                    await context.OrganizationUsers.AddAsync(clientUser);
                    logger.LogInformation("Client Admin OrganizationUser created and linked.");
                }
                else if (!existingClient.HasRole(Role.OrganizationAdministrator.Id))
                {
                    existingClient.AssignRole(Role.OrganizationAdministrator.Id);
                    logger.LogInformation("Assigned OrganizationAdministrator role to existing Client user.");
                }
            }

            if (operatorAuthUser != null)
            {
                var existingOperator = await context.OrganizationUsers
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(ou => ou.SupabaseUserId == operatorAuthUser.Id);
                
                if (existingOperator == null)
                {
                    var operatorUser = OrganizationUser.Create(
                        userId: Guid.NewGuid(),
                        firstName: "John",
                        lastName: "Operator",
                        email: "operator@test.com",
                        organizationId: testOrgId,
                        phoneNumber: "+48 123 456 004");
                    
                    typeof(OrganizationUser).GetProperty("SupabaseUserId")!.SetValue(operatorUser, operatorAuthUser.Id);
                    operatorUser.Activate();
                    operatorUser.AssignRole(Role.Operator.Id);
                    
                    await context.OrganizationUsers.AddAsync(operatorUser);
                    logger.LogInformation("Operator OrganizationUser created and linked.");
                }
                else if (!existingOperator.HasRole(Role.Operator.Id))
                {
                    existingOperator.AssignRole(Role.Operator.Id);
                    logger.LogInformation("Assigned Operator role to existing Operator user.");
                }
            }

            // Create sample machines
            var machine1SerialNumber = "FM3000-001";
            var existingMachine1 = await context.Machines
                .FirstOrDefaultAsync(m => m.SerialNumber == machine1SerialNumber);
            
            if (existingMachine1 == null)
            {
                var machine1 = Machine.Create(
                    organizationId: testOrgId,
                    serialNumber: machine1SerialNumber,
                    brand: "Baumalog",
                    model: "FlowMaster 3000",
                    location: "Production Hall A",
                    createdBy: null);
                
                await context.Machines.AddAsync(machine1);
                logger.LogInformation("Machine FM3000-001 created.");
            }

            var machine2SerialNumber = "FM3000-002";
            var existingMachine2 = await context.Machines
                .FirstOrDefaultAsync(m => m.SerialNumber == machine2SerialNumber);
            
            if (existingMachine2 == null)
            {
                var machine2 = Machine.Create(
                    organizationId: testOrgId,
                    serialNumber: machine2SerialNumber,
                    brand: "Baumalog",
                    model: "FlowMaster 3000",
                    location: "Production Hall B",
                    createdBy: null);
                
                await context.Machines.AddAsync(machine2);
                logger.LogInformation("Machine FM3000-002 created.");
            }

            await context.SaveChangesAsync();
            logger.LogInformation("Test users and machines seeded successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while seeding test users.");
            // Don't throw - allow application to start even if test user seeding fails
        }
    }

    // DTO for reading auth.users
    private class AuthUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    private static Organization CreateOrganization(
        string name,
        string email,
        string phone,
        string address,
        string city,
        string postalCode,
        string country)
    {
        return Organization.Create(
            name: name,
            email: email,
            phone: phone,
            address: address,
            city: city,
            postalCode: postalCode,
            country: country);
    }
}
