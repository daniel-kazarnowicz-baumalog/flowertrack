using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Tests.ValueObjects;

/// <summary>
/// Unit tests for Email Value Object
/// </summary>
public class EmailTests
{
    #region Create Tests

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.user@example.com")]
    [InlineData("user+tag@example.co.uk")]
    [InlineData("user_name@example-domain.com")]
    [InlineData("123@numbers.com")]
    public void Create_WithValidEmail_ShouldCreateEmail(string emailAddress)
    {
        // Act
        var email = Email.Create(emailAddress);

        // Assert
        Assert.NotNull(email);
        Assert.Equal(emailAddress.ToLowerInvariant(), email.Value);
    }

    [Fact]
    public void Create_WithUppercase_ShouldNormalizeToLowercase()
    {
        // Arrange
        var input = "USER@EXAMPLE.COM";

        // Act
        var email = Email.Create(input);

        // Assert
        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void Create_WithWhitespace_ShouldTrimAndNormalize()
    {
        // Arrange
        var input = "  user@example.com  ";

        // Act
        var email = Email.Create(input);

        // Assert
        Assert.Equal("user@example.com", email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespace_ShouldThrowArgumentException(string input)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(input));
    }

    [Fact]
    public void Create_WithNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(null!));
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user @example.com")]
    [InlineData("user@example")]
    [InlineData("user..name@example.com")]
    public void Create_WithInvalidFormat_ShouldThrowArgumentException(string input)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(input));
        Assert.Contains("Invalid email format", exception.Message);
    }

    [Fact]
    public void Create_WithEmailTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@example.com"; // > 255 chars

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(longEmail));
        Assert.Contains("Email cannot exceed 255 characters", exception.Message);
    }

    #endregion

    #region Parse Tests

    [Fact]
    public void Parse_WithValidEmail_ShouldParse()
    {
        // Arrange
        var input = "user@example.com";

        // Act
        var email = Email.Parse(input);

        // Assert
        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void Parse_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var input = "invalid-email";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Parse(input));
    }

    #endregion

    #region TryParse Tests

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.user@subdomain.example.com")]
    [InlineData("USER@EXAMPLE.COM")]
    public void TryParse_WithValidEmail_ShouldReturnTrue(string input)
    {
        // Act
        var success = Email.TryParse(input, out var result);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal(input.ToLowerInvariant().Trim(), result.Value);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParse_WithInvalidEmail_ShouldReturnFalse(string? input)
    {
        // Act
        var success = Email.TryParse(input, out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void TryParse_WithEmailTooLong_ShouldReturnFalse()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@example.com";

        // Act
        var success = Email.TryParse(longEmail, out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameEmail_ShouldBeEqual()
    {
        // Arrange
        var email1 = Email.Create("user@example.com");
        var email2 = Email.Create("user@example.com");

        // Act & Assert
        Assert.Equal(email1, email2);
        Assert.True(email1 == email2);
    }

    [Fact]
    public void Equals_WithSameEmailDifferentCase_ShouldBeEqual()
    {
        // Arrange
        var email1 = Email.Create("USER@EXAMPLE.COM");
        var email2 = Email.Create("user@example.com");

        // Act & Assert
        Assert.Equal(email1, email2);
    }

    [Fact]
    public void Equals_WithDifferentEmails_ShouldNotBeEqual()
    {
        // Arrange
        var email1 = Email.Create("user1@example.com");
        var email2 = Email.Create("user2@example.com");

        // Act & Assert
        Assert.NotEqual(email1, email2);
        Assert.True(email1 != email2);
    }

    [Fact]
    public void GetHashCode_WithSameEmail_ShouldBeSame()
    {
        // Arrange
        var email1 = Email.Create("user@example.com");
        var email2 = Email.Create("USER@EXAMPLE.COM");

        // Act & Assert
        Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ShouldReturnNormalizedValue()
    {
        // Arrange
        var email = Email.Create("USER@EXAMPLE.COM");

        // Act
        var result = email.ToString();

        // Assert
        Assert.Equal("user@example.com", result);
    }

    #endregion

    #region Conversion Tests

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var email = Email.Create("user@example.com");

        // Act
        string value = email;

        // Assert
        Assert.Equal("user@example.com", value);
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldWork()
    {
        // Arrange
        var value = "user@example.com";

        // Act
        var email = (Email)value;

        // Assert
        Assert.Equal("user@example.com", email.Value);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Create_WithMaxLength_ShouldSucceed()
    {
        // Arrange - Create a 255-character email
        // "@example.com" is 12 characters, so local part should be 243 characters
        var localPart = new string('a', 243);
        var email = $"{localPart}@example.com"; // Exactly 255 chars

        // Act
        var result = Email.Create(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(255, result.Value.Length);
    }

    [Theory]
    [InlineData("a@b.co")]
    [InlineData("test@test.museum")]
    [InlineData("user@subdomain.domain.co.uk")]
    public void Create_WithVariousTLDs_ShouldSucceed(string emailAddress)
    {
        // Act
        var email = Email.Create(emailAddress);

        // Assert
        Assert.NotNull(email);
        Assert.Equal(emailAddress.ToLowerInvariant(), email.Value);
    }

    #endregion
}
