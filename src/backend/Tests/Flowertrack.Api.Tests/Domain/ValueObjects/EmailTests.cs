using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Api.Tests.Domain.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@example.com")]
    [InlineData("user+tag@example.com")]
    [InlineData("user_name@example.com")]
    [InlineData("user123@example123.com")]
    [InlineData("test@subdomain.example.com")]
    [InlineData("a@b.co")]
    public void Create_WithValidEmail_ShouldSucceed(string emailValue)
    {
        // Act
        var email = Email.Create(emailValue);

        // Assert
        Assert.NotNull(email);
        Assert.Equal(emailValue.ToLowerInvariant(), email.Value);
    }

    [Fact]
    public void Create_WithUppercaseEmail_ShouldNormalizeToLowercase()
    {
        // Arrange
        var emailValue = "TEST@EXAMPLE.COM";

        // Act
        var email = Email.Create(emailValue);

        // Assert
        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void Create_WithEmailWithWhitespace_ShouldTrimAndNormalize()
    {
        // Arrange
        var emailValue = "  test@example.com  ";

        // Act
        var email = Email.Create(emailValue);

        // Assert
        Assert.Equal("test@example.com", email.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrEmpty_ShouldThrowArgumentException(string? emailValue)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(emailValue!));
        Assert.Contains("cannot be null or empty", exception.Message);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test")]
    [InlineData("test@.com")]
    [InlineData("test@example")]
    [InlineData("test @example.com")]
    [InlineData("test@exam ple.com")]
    [InlineData("test@@example.com")]
    [InlineData("test..name@example.com")]
    public void Create_WithInvalidFormat_ShouldThrowArgumentException(string emailValue)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(emailValue));
        Assert.Contains("Invalid email format", exception.Message);
    }

    [Fact]
    public void Create_WithEmailExceedingMaxLength_ShouldThrowArgumentException()
    {
        // Arrange - Create a string longer than 255 characters
        var localPart = new string('a', 244);
        var emailValue = $"{localPart}@example.com"; // 244 + 1 + 11 = 256 chars > 255

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Email.Create(emailValue));
        Assert.Contains("cannot exceed 255 characters", exception.Message);
    }

    [Fact]
    public void Create_WithEmailAtMaxLength_ShouldSucceed()
    {
        // Arrange - Create exactly 255 characters
        var localPart = new string('a', 242); // 242 + "@" + "example.com" (11) = 254
        var emailValue = $"{localPart}@example.com";

        // Act
        var email = Email.Create(emailValue);

        // Assert
        Assert.NotNull(email);
        Assert.True(email.Value.Length <= 255);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@example.com")]
    public void Parse_WithValidEmail_ShouldSucceed(string emailValue)
    {
        // Act
        var email = Email.Parse(emailValue);

        // Assert
        Assert.NotNull(email);
        Assert.Equal(emailValue.ToLowerInvariant(), email.Value);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("USER@EXAMPLE.COM", true)]
    [InlineData("  test@example.com  ", true)]
    public void TryParse_WithValidEmail_ShouldReturnTrue(string emailValue, bool expectedResult)
    {
        // Act
        var result = Email.TryParse(emailValue, out var email);

        // Assert
        Assert.Equal(expectedResult, result);
        Assert.NotNull(email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    public void TryParse_WithInvalidEmail_ShouldReturnFalse(string? emailValue)
    {
        // Act
        var result = Email.TryParse(emailValue, out var email);

        // Assert
        Assert.False(result);
        Assert.Null(email);
    }

    [Fact]
    public void TryParse_WithEmailExceedingMaxLength_ShouldReturnFalse()
    {
        // Arrange
        var localPart = new string('a', 244);
        var emailValue = $"{localPart}@example.com"; // 244 + 1 + 11 = 256 chars > 255

        // Act
        var result = Email.TryParse(emailValue, out var email);

        // Assert
        Assert.False(result);
        Assert.Null(email);
    }

    [Fact]
    public void ToString_ShouldReturnNormalizedValue()
    {
        // Arrange
        var email = Email.Create("TEST@EXAMPLE.COM");

        // Act
        var result = email.ToString();

        // Assert
        Assert.Equal("test@example.com", result);
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");

        // Assert
        Assert.Equal(email1, email2);
        Assert.True(email1 == email2);
        Assert.False(email1 != email2);
        Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
    }

    [Fact]
    public void Equality_WithSameValuesButDifferentCase_ShouldBeEqual()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("TEST@EXAMPLE.COM");

        // Assert - Should be equal because normalization converts to lowercase
        Assert.Equal(email1, email2);
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com");
        var email2 = Email.Create("test2@example.com");

        // Assert
        Assert.NotEqual(email1, email2);
        Assert.False(email1 == email2);
        Assert.True(email1 != email2);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act
        string value = email;

        // Assert
        Assert.Equal("test@example.com", value);
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldWork()
    {
        // Arrange
        var value = "test@example.com";

        // Act
        var email = (Email)value;

        // Assert
        Assert.Equal(value, email.Value);
    }
}
