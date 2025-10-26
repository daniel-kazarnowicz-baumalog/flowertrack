using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Tests.ValueObjects;

/// <summary>
/// Unit tests for MachineApiKey Value Object
/// </summary>
public class MachineApiKeyTests
{
    #region Generate Tests

    [Fact]
    public void Generate_ShouldCreateValidApiKey()
    {
        // Act
        var apiKey = MachineApiKey.Generate();

        // Assert
        Assert.NotNull(apiKey);
        Assert.NotNull(apiKey.Value);
        Assert.StartsWith("mch_", apiKey.Value);
        Assert.True(apiKey.Value.Length >= 36); // mch_ (4) + token (32+)
    }

    [Fact]
    public void Generate_MultipleTimes_ShouldCreateUniqueTokens()
    {
        // Act
        var apiKey1 = MachineApiKey.Generate();
        var apiKey2 = MachineApiKey.Generate();
        var apiKey3 = MachineApiKey.Generate();

        // Assert
        Assert.NotEqual(apiKey1.Value, apiKey2.Value);
        Assert.NotEqual(apiKey2.Value, apiKey3.Value);
        Assert.NotEqual(apiKey1.Value, apiKey3.Value);
    }

    [Fact]
    public void Generate_ShouldCreateSecureRandomToken()
    {
        // Act - Generate multiple tokens
        var tokens = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            var apiKey = MachineApiKey.Generate();
            tokens.Add(apiKey.Value);
        }

        // Assert - All tokens should be unique (no collisions in 100 generations)
        Assert.Equal(100, tokens.Count);
    }

    [Fact]
    public void Generate_ShouldCreateTokenWithValidFormat()
    {
        // Act
        var apiKey = MachineApiKey.Generate();

        // Assert
        Assert.Matches(@"^mch_[a-zA-Z0-9-]{32,40}$", apiKey.Value);
    }

    #endregion

    #region Create Tests

    [Theory]
    [InlineData("mch_abcdefghijklmnopqrstuvwxyz123456")]
    [InlineData("mch_12345678901234567890123456789012")]
    [InlineData("mch_ABCD-EFGH-1234-5678-90ab-cdef1234")]
    public void Create_WithValidToken_ShouldCreateApiKey(string token)
    {
        // Act
        var apiKey = MachineApiKey.Create(token);

        // Assert
        Assert.NotNull(apiKey);
        Assert.Equal(token, apiKey.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespace_ShouldThrowArgumentException(string token)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => MachineApiKey.Create(token));
    }

    [Fact]
    public void Create_WithNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => MachineApiKey.Create(null!));
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("mch_short")]
    [InlineData("mch_")]
    [InlineData("api_12345678901234567890123456789012")]
    [InlineData("mch_123")]
    [InlineData("mch_invalid@token#format$here%test")]
    public void Create_WithInvalidFormat_ShouldThrowArgumentException(string token)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => MachineApiKey.Create(token));
        // Just verify that an exception is thrown with a message (not checking exact text)
        Assert.NotEmpty(exception.Message);
    }

    #endregion

    #region TryCreate Tests

    [Fact]
    public void TryCreate_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var token = "mch_abcdefghijklmnopqrstuvwxyz123456";

        // Act
        var success = MachineApiKey.TryCreate(token, out var result);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal(token, result.Value);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("mch_short")]
    [InlineData("")]
    [InlineData(null)]
    public void TryCreate_WithInvalidToken_ShouldReturnFalse(string? token)
    {
        // Act
        var success = MachineApiKey.TryCreate(token, out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameToken_ShouldBeEqual()
    {
        // Arrange
        var token = "mch_abcdefghijklmnopqrstuvwxyz123456";
        var apiKey1 = MachineApiKey.Create(token);
        var apiKey2 = MachineApiKey.Create(token);

        // Act & Assert
        Assert.Equal(apiKey1, apiKey2);
        Assert.True(apiKey1 == apiKey2);
    }

    [Fact]
    public void Equals_WithDifferentTokens_ShouldNotBeEqual()
    {
        // Arrange
        var apiKey1 = MachineApiKey.Generate();
        var apiKey2 = MachineApiKey.Generate();

        // Act & Assert
        Assert.NotEqual(apiKey1, apiKey2);
        Assert.True(apiKey1 != apiKey2);
    }

    [Fact]
    public void GetHashCode_WithSameToken_ShouldBeSame()
    {
        // Arrange
        var token = "mch_abcdefghijklmnopqrstuvwxyz123456";
        var apiKey1 = MachineApiKey.Create(token);
        var apiKey2 = MachineApiKey.Create(token);

        // Act & Assert
        Assert.Equal(apiKey1.GetHashCode(), apiKey2.GetHashCode());
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ShouldReturnTokenValue()
    {
        // Arrange
        var token = "mch_abcdefghijklmnopqrstuvwxyz123456";
        var apiKey = MachineApiKey.Create(token);

        // Act
        var result = apiKey.ToString();

        // Assert
        Assert.Equal(token, result);
    }

    #endregion

    #region Conversion Tests

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var token = "mch_abcdefghijklmnopqrstuvwxyz123456";
        var apiKey = MachineApiKey.Create(token);

        // Act
        string value = apiKey;

        // Assert
        Assert.Equal(token, value);
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldWork()
    {
        // Arrange
        var token = "mch_abcdefghijklmnopqrstuvwxyz123456";

        // Act
        var apiKey = (MachineApiKey)token;

        // Assert
        Assert.Equal(token, apiKey.Value);
    }

    #endregion

    #region Security Tests

    [Fact]
    public void Generate_TokenLength_ShouldBeConsistent()
    {
        // Act
        var tokens = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            var apiKey = MachineApiKey.Generate();
            tokens.Add(apiKey.Value);
        }

        // Assert - All generated tokens should have the same length
        var firstLength = tokens[0].Length;
        Assert.All(tokens, token => Assert.Equal(firstLength, token.Length));
    }

    [Fact]
    public void Generate_ShouldNotContainPredictablePatterns()
    {
        // Act
        var apiKey1 = MachineApiKey.Generate();
        var apiKey2 = MachineApiKey.Generate();

        // Assert - Tokens should not share common substrings (except prefix)
        var token1 = apiKey1.Value.Substring(4); // Remove prefix
        var token2 = apiKey2.Value.Substring(4);

        // Check that tokens are different
        Assert.NotEqual(token1, token2);

        // Check that first 10 characters are different (very unlikely to be same in secure random)
        Assert.NotEqual(token1.Substring(0, 10), token2.Substring(0, 10));
    }

    [Fact]
    public void Generate_ShouldOnlyContainAllowedCharacters()
    {
        // Act
        var apiKey = MachineApiKey.Generate();
        var token = apiKey.Value.Substring(4); // Remove "mch_" prefix

        // Assert - Should only contain alphanumeric and dash
        Assert.All(token, c => Assert.True(char.IsLetterOrDigit(c) || c == '-'));
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Create_WithExactly32CharToken_ShouldSucceed()
    {
        // Arrange
        var token = "mch_" + new string('a', 32);

        // Act
        var apiKey = MachineApiKey.Create(token);

        // Assert
        Assert.NotNull(apiKey);
        Assert.Equal(36, apiKey.Value.Length); // 4 (mch_) + 32
    }

    [Fact]
    public void Create_WithExactly40CharToken_ShouldSucceed()
    {
        // Arrange
        var token = "mch_" + new string('a', 40);

        // Act
        var apiKey = MachineApiKey.Create(token);

        // Assert
        Assert.NotNull(apiKey);
        Assert.Equal(44, apiKey.Value.Length); // 4 (mch_) + 40
    }

    [Fact]
    public void Create_With31CharToken_ShouldFail()
    {
        // Arrange
        var token = "mch_" + new string('a', 31);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => MachineApiKey.Create(token));
    }

    [Fact]
    public void Create_With41CharToken_ShouldFail()
    {
        // Arrange
        var token = "mch_" + new string('a', 41);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => MachineApiKey.Create(token));
    }

    #endregion
}
