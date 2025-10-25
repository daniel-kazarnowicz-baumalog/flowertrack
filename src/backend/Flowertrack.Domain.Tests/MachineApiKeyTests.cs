using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Tests;

public class MachineApiKeyTests
{
    [Fact]
    public void Generate_CreatesNewToken()
    {
        // Act
        var apiKey = MachineApiKey.Generate();

        // Assert
        Assert.NotNull(apiKey);
        Assert.NotNull(apiKey.Value);
        Assert.True(apiKey.Value.Length >= 32);
    }

    [Fact]
    public void Generate_CreatesDifferentTokensEachTime()
    {
        // Act
        var apiKey1 = MachineApiKey.Generate();
        var apiKey2 = MachineApiKey.Generate();

        // Assert
        Assert.NotEqual(apiKey1.Value, apiKey2.Value);
    }

    [Fact]
    public void Generate_SetsGeneratedAtToCurrentTime()
    {
        // Arrange
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var apiKey = MachineApiKey.Generate();

        // Assert
        var afterGeneration = DateTime.UtcNow;
        Assert.InRange(apiKey.GeneratedAt, beforeGeneration, afterGeneration);
    }

    [Fact]
    public void FromValue_WithValidToken_CreatesApiKey()
    {
        // Arrange
        var token = new string('A', 32);

        // Act
        var apiKey = MachineApiKey.FromValue(token);

        // Assert
        Assert.NotNull(apiKey);
        Assert.Equal(token, apiKey.Value);
    }

    [Fact]
    public void FromValue_WithEmptyToken_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            MachineApiKey.FromValue(""));

        Assert.Contains("API token value cannot be empty", exception.Message);
    }

    [Fact]
    public void FromValue_WithWhitespaceToken_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            MachineApiKey.FromValue("   "));

        Assert.Contains("API token value cannot be empty", exception.Message);
    }

    [Fact]
    public void FromValue_WithTokenTooShort_ThrowsArgumentException()
    {
        // Arrange
        var shortToken = "ABC123";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            MachineApiKey.FromValue(shortToken));

        Assert.Contains("API token must be at least 32 characters", exception.Message);
    }

    [Fact]
    public void Equals_WithSameValue_ReturnsTrue()
    {
        // Arrange
        var token = new string('A', 32);
        var apiKey1 = MachineApiKey.FromValue(token);
        var apiKey2 = MachineApiKey.FromValue(token);

        // Act & Assert
        Assert.True(apiKey1.Equals(apiKey2));
        Assert.True(apiKey1 == apiKey2);
    }

    [Fact]
    public void Equals_WithDifferentValue_ReturnsFalse()
    {
        // Arrange
        var apiKey1 = MachineApiKey.Generate();
        var apiKey2 = MachineApiKey.Generate();

        // Act & Assert
        Assert.False(apiKey1.Equals(apiKey2));
        Assert.True(apiKey1 != apiKey2);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var apiKey = MachineApiKey.Generate();

        // Act & Assert
        Assert.False(apiKey.Equals(null));
    }

    [Fact]
    public void GetHashCode_WithSameValue_ReturnsSameHash()
    {
        // Arrange
        var token = new string('A', 32);
        var apiKey1 = MachineApiKey.FromValue(token);
        var apiKey2 = MachineApiKey.FromValue(token);

        // Act & Assert
        Assert.Equal(apiKey1.GetHashCode(), apiKey2.GetHashCode());
    }

    [Fact]
    public void ToString_MasksTokenValue()
    {
        // Arrange
        var token = "ABCDEFGHIJ1234567890KLMNOPQRST12";
        var apiKey = MachineApiKey.FromValue(token);

        // Act
        var stringRepresentation = apiKey.ToString();

        // Assert
        Assert.StartsWith("ABCDEFGH", stringRepresentation);
        Assert.EndsWith("...", stringRepresentation);
        Assert.DoesNotContain(token, stringRepresentation);
    }

    [Fact]
    public void OperatorEquals_WithBothNull_ReturnsTrue()
    {
        // Arrange
        MachineApiKey? apiKey1 = null;
        MachineApiKey? apiKey2 = null;

        // Act & Assert
        Assert.True(apiKey1 == apiKey2);
    }

    [Fact]
    public void OperatorEquals_WithOneNull_ReturnsFalse()
    {
        // Arrange
        var apiKey = MachineApiKey.Generate();

        // Act & Assert
        Assert.False(apiKey == null);
        Assert.False(null == apiKey);
    }
}
