using Flowertrack.Infrastructure.Configuration;
using Flowertrack.Infrastructure.Supabase;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Flowertrack.Infrastructure.Tests.Supabase;

/// <summary>
/// Unit tests for SupabaseClientService
/// </summary>
public class SupabaseClientServiceTests
{
    private readonly Mock<ILogger<SupabaseClientService>> _mockLogger;
    private readonly SupabaseOptions _supabaseOptions;

    public SupabaseClientServiceTests()
    {
        _mockLogger = new Mock<ILogger<SupabaseClientService>>();
        
        // Create valid test configuration
        _supabaseOptions = new SupabaseOptions
        {
            Url = "https://test-project.supabase.co",
            AnonKey = "test-anon-key",
            ServiceKey = "test-service-key",
            JwtSecret = "test-jwt-secret-must-be-at-least-32-characters-long"
        };
    }

    [Fact]
    public void Constructor_WithValidOptions_ShouldNotThrow()
    {
        // Arrange
        var options = Options.Create(_supabaseOptions);

        // Act & Assert
        var exception = Record.Exception(() => new SupabaseClientService(options, _mockLogger.Object));
        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new SupabaseClientService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var options = Options.Create(_supabaseOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new SupabaseClientService(options, null!));
    }

    [Fact]
    public void GetClient_ShouldReturnClient()
    {
        // Arrange
        var options = Options.Create(_supabaseOptions);
        var service = new SupabaseClientService(options, _mockLogger.Object);

        // Act
        var client = service.GetClient();

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void GetClient_CalledMultipleTimes_ShouldReturnSameInstance()
    {
        // Arrange
        var options = Options.Create(_supabaseOptions);
        var service = new SupabaseClientService(options, _mockLogger.Object);

        // Act
        var client1 = service.GetClient();
        var client2 = service.GetClient();

        // Assert
        Assert.Same(client1, client2);
    }

    [Fact]
    public void Auth_ShouldReturnAuthClient()
    {
        // Arrange
        var options = Options.Create(_supabaseOptions);
        var service = new SupabaseClientService(options, _mockLogger.Object);

        // Act
        var auth = service.Auth;

        // Assert
        Assert.NotNull(auth);
    }

    [Fact]
    public void Storage_ShouldReturnStorageClient()
    {
        // Arrange
        var options = Options.Create(_supabaseOptions);
        var service = new SupabaseClientService(options, _mockLogger.Object);

        // Act
        var storage = service.Storage;

        // Assert
        Assert.NotNull(storage);
    }

    [Fact]
    public void GetClient_ShouldLogInformation()
    {
        // Arrange
        var options = Options.Create(_supabaseOptions);
        var service = new SupabaseClientService(options, _mockLogger.Object);

        // Act
        var client = service.GetClient();

        // Assert - verify that logging occurred
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Initializing Supabase client")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
