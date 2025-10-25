using Flowertrack.Domain.Entities.Organizations;
using Flowertrack.Domain.Enums;
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

            // Check if data already exists
            if (await context.Organizations.AnyAsync())
            {
                logger.LogInformation("Database already contains data. Skipping seed.");
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
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
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
