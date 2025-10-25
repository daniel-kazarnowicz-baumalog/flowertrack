using Flowertrack.Api.Domain.ValueObjects;

namespace Flowertrack.Api.Tests.Domain.ValueObjects;

public class MachineApiKeyTests
{
    [Fact]
    public void Generate_ShouldCreateValidApiKey()
    {
        // Act
        var apiKey = MachineApiKey.Generate();

        // Assert
        Assert.NotNull(apiKey);
        Assert.StartsWith("mch_", apiKey.Value);
        Assert.True(apiKey.Value.Length >= 36 && apiKey.Value.Length <= 40);
    }

    [Fact]
    public void Generate_ShouldCreateUniqueApiKeys()
    {
        // Act
        var apiKey1 = MachineApiKey.Generate();
        var apiKey2 = MachineApiKey.Generate();
        var apiKey3 = MachineApiKey.Generate();

        // Assert
        Assert.NotEqual(apiKey1.Value, apiKey2.Value);
        Assert.NotEqual(apiKey1.Value, apiKey3.Value);
        Assert.NotEqual(apiKey2.Value, apiKey3.Value);
    }

    [Fact]
    public void Generate_MultipleTimes_ShouldNotCreatePredictablePattern()
    {
        // Arrange
        var apiKeys = new HashSet<string>();

        // Act - Generate 100 keys to check for patterns
        for (int i = 0; i < 100; i++)
        {
            var apiKey = MachineApiKey.Generate();
            apiKeys.Add(apiKey.Value);
        }

        // Assert - All keys should be unique
        Assert.Equal(100, apiKeys.Count);
    }

    [Theory]
    [InlineData("mch_abcdefghijklmnopqrstuvwxyzABCDEF")]
    [InlineData("mch_12345678901234567890123456789012")]
    [InlineData("mch_abc123def456ghi789jkl012mno345pq")]
    public void Create_WithValidFormat_ShouldSucceed(string value)
    {
        // Act
        var apiKey = MachineApiKey.Create(value);

        // Assert
        Assert.NotNull(apiKey);
        Assert.Equal(value, apiKey.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrEmpty_ShouldThrowArgumentException(string? value)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => MachineApiKey.Create(value!));
        Assert.Contains("cannot be null or empty", exception.Message);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("machine_key")]
    [InlineData("api_key123")]
    [InlineData("mch_short")] // Too short
    [InlineData("MCH_UPPERCASE")] // Wrong prefix case
    [InlineData("mch-withdashinsteadofunderscore")]
    [InlineData("mch_")] // Just prefix
    public void Create_WithInvalidFormat_ShouldThrowArgumentException(string value)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => MachineApiKey.Create(value));
        Assert.Contains("Machine API key", exception.Message);
    }

    [Fact]
    public void Create_WithoutPrefix_ShouldThrowArgumentException()
    {
        // Arrange
        var value = "abcdefghijklmnopqrstuvwxyzABCDEF012";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => MachineApiKey.Create(value));
        Assert.Contains("must start with 'mch_'", exception.Message);
    }

    [Theory]
    [InlineData("mch_abcdefghijklmnopqrstuvwxyzABCDEF", true)]
    [InlineData("mch_12345678901234567890123456789012", true)]
    public void TryCreate_WithValidFormat_ShouldReturnTrue(string value, bool expectedResult)
    {
        // Act
        var result = MachineApiKey.TryCreate(value, out var apiKey);

        // Assert
        Assert.Equal(expectedResult, result);
        Assert.NotNull(apiKey);
        Assert.Equal(value, apiKey?.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid")]
    [InlineData("mch_short")]
    [InlineData("wrong_prefix_abcdefghijklmnopqrs")]
    public void TryCreate_WithInvalidFormat_ShouldReturnFalse(string? value)
    {
        // Act
        var result = MachineApiKey.TryCreate(value, out var apiKey);

        // Assert
        Assert.False(result);
        Assert.Null(apiKey);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var value = "mch_abcdefghijklmnopqrstuvwxyzABCDEF";
        var apiKey = MachineApiKey.Create(value);

        // Act
        var result = apiKey.ToString();

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var value = "mch_abcdefghijklmnopqrstuvwxyzABCDEF";
        var apiKey1 = MachineApiKey.Create(value);
        var apiKey2 = MachineApiKey.Create(value);

        // Assert
        Assert.Equal(apiKey1, apiKey2);
        Assert.True(apiKey1 == apiKey2);
        Assert.False(apiKey1 != apiKey2);
        Assert.Equal(apiKey1.GetHashCode(), apiKey2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var apiKey1 = MachineApiKey.Generate();
        var apiKey2 = MachineApiKey.Generate();

        // Assert
        Assert.NotEqual(apiKey1, apiKey2);
        Assert.False(apiKey1 == apiKey2);
        Assert.True(apiKey1 != apiKey2);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var value = "mch_abcdefghijklmnopqrstuvwxyzABCDEF";
        var apiKey = MachineApiKey.Create(value);

        // Act
        string convertedValue = apiKey;

        // Assert
        Assert.Equal(value, convertedValue);
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldWork()
    {
        // Arrange
        var value = "mch_abcdefghijklmnopqrstuvwxyzABCDEF";

        // Act
        var apiKey = (MachineApiKey)value;

        // Assert
        Assert.Equal(value, apiKey.Value);
    }

    [Fact]
    public void Generate_ShouldUseSecureRandomGeneration()
    {
        // Arrange - Generate multiple keys and check that they don't follow a predictable pattern
        var keys = new List<string>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            keys.Add(MachineApiKey.Generate().Value);
        }

        // Assert - Check that no key is a substring of another (would indicate pattern)
        for (int i = 0; i < keys.Count; i++)
        {
            for (int j = i + 1; j < keys.Count; j++)
            {
                var key1WithoutPrefix = keys[i].Substring(4);
                var key2WithoutPrefix = keys[j].Substring(4);

                Assert.DoesNotContain(key2WithoutPrefix.Substring(0, Math.Min(10, key2WithoutPrefix.Length)), key1WithoutPrefix);
            }
        }
    }

    [Fact]
    public void Generate_ShouldProduceConsistentLength()
    {
        // Act
        var keys = Enumerable.Range(0, 20)
            .Select(_ => MachineApiKey.Generate())
            .ToList();

        // Assert - All keys should have the same length
        var expectedLength = keys.First().Value.Length;
        Assert.All(keys, key => Assert.Equal(expectedLength, key.Value.Length));
    }
}
