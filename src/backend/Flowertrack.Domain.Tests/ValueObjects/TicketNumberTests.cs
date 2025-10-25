using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Tests.ValueObjects;

/// <summary>
/// Unit tests for TicketNumber Value Object
/// </summary>
public class TicketNumberTests
{
    #region Create Tests

    [Fact]
    public void Create_WithValidYearAndSequential_ShouldCreateTicketNumber()
    {
        // Arrange
        var year = 2025;
        var sequential = 1;

        // Act
        var ticketNumber = TicketNumber.Create(year, sequential);

        // Assert
        Assert.NotNull(ticketNumber);
        Assert.Equal(year, ticketNumber.Year);
        Assert.Equal(sequential, ticketNumber.Sequential);
        Assert.Equal("TICK-2025-00001", ticketNumber.Value);
    }

    [Fact]
    public void Create_WithMaxSequential_ShouldCreateTicketNumber()
    {
        // Arrange
        var year = 2025;
        var sequential = 99999;

        // Act
        var ticketNumber = TicketNumber.Create(year, sequential);

        // Assert
        Assert.Equal("TICK-2025-99999", ticketNumber.Value);
    }

    [Theory]
    [InlineData(2020)]
    [InlineData(2025)]
    public void Create_WithValidYears_ShouldSucceed(int year)
    {
        // Act
        var ticketNumber = TicketNumber.Create(year, 1);

        // Assert
        Assert.Equal(year, ticketNumber.Year);
    }

    [Fact]
    public void Create_WithYearTooLow_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TicketNumber.Create(2019, 1));

        Assert.Contains("Year must be between", exception.Message);
    }

    [Fact]
    public void Create_WithYearTooHigh_ShouldThrowArgumentException()
    {
        // Arrange
        var futureYear = DateTime.UtcNow.Year + 2;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TicketNumber.Create(futureYear, 1));

        Assert.Contains("Year must be between", exception.Message);
    }

    [Fact]
    public void Create_WithSequentialTooLow_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TicketNumber.Create(2025, 0));

        Assert.Contains("Sequential must be between", exception.Message);
    }

    [Fact]
    public void Create_WithSequentialTooHigh_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TicketNumber.Create(2025, 100000));

        Assert.Contains("Sequential must be between", exception.Message);
    }

    #endregion

    #region Parse Tests

    [Theory]
    [InlineData("TICK-2025-00001", 2025, 1)]
    [InlineData("TICK-2024-12345", 2024, 12345)]
    [InlineData("TICK-2023-99999", 2023, 99999)]
    public void Parse_WithValidFormat_ShouldParseCorrectly(string input, int expectedYear, int expectedSequential)
    {
        // Act
        var ticketNumber = TicketNumber.Parse(input);

        // Assert
        Assert.Equal(expectedYear, ticketNumber.Year);
        Assert.Equal(expectedSequential, ticketNumber.Sequential);
        Assert.Equal(input, ticketNumber.Value);
    }

    [Theory]
    [InlineData("TICK-2025-1")]
    [InlineData("TICK-25-00001")]
    [InlineData("TICKET-2025-00001")]
    [InlineData("TCK-2025-00001")]
    [InlineData("TICK-2025-000001")]
    [InlineData("TICK-2025-0001")]
    public void Parse_WithInvalidFormat_ShouldThrowArgumentException(string input)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TicketNumber.Parse(input));

        Assert.Contains("Invalid ticket number format", exception.Message);
    }

    [Fact]
    public void Parse_WithNullOrEmpty_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => TicketNumber.Parse(null!));
        Assert.Throws<ArgumentException>(() => TicketNumber.Parse(""));
        Assert.Throws<ArgumentException>(() => TicketNumber.Parse("   "));
    }

    #endregion

    #region TryParse Tests

    [Fact]
    public void TryParse_WithValidFormat_ShouldReturnTrue()
    {
        // Arrange
        var input = "TICK-2025-00001";

        // Act
        var success = TicketNumber.TryParse(input, out var result);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal(2025, result.Year);
        Assert.Equal(1, result.Sequential);
    }

    [Theory]
    [InlineData("TICK-2025-1")]
    [InlineData("INVALID")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParse_WithInvalidFormat_ShouldReturnFalse(string? input)
    {
        // Act
        var success = TicketNumber.TryParse(input, out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var ticketNumber1 = TicketNumber.Create(2025, 1);
        var ticketNumber2 = TicketNumber.Create(2025, 1);

        // Act & Assert
        Assert.Equal(ticketNumber1, ticketNumber2);
        Assert.True(ticketNumber1 == ticketNumber2);
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var ticketNumber1 = TicketNumber.Create(2025, 1);
        var ticketNumber2 = TicketNumber.Create(2025, 2);

        // Act & Assert
        Assert.NotEqual(ticketNumber1, ticketNumber2);
        Assert.True(ticketNumber1 != ticketNumber2);
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldBeSame()
    {
        // Arrange
        var ticketNumber1 = TicketNumber.Create(2025, 1);
        var ticketNumber2 = TicketNumber.Create(2025, 1);

        // Act & Assert
        Assert.Equal(ticketNumber1.GetHashCode(), ticketNumber2.GetHashCode());
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ShouldReturnFormattedValue()
    {
        // Arrange
        var ticketNumber = TicketNumber.Create(2025, 123);

        // Act
        var result = ticketNumber.ToString();

        // Assert
        Assert.Equal("TICK-2025-00123", result);
    }

    #endregion

    #region Conversion Tests

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var ticketNumber = TicketNumber.Create(2025, 1);

        // Act
        string value = ticketNumber;

        // Assert
        Assert.Equal("TICK-2025-00001", value);
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldWork()
    {
        // Arrange
        var value = "TICK-2025-00001";

        // Act
        var ticketNumber = (TicketNumber)value;

        // Assert
        Assert.Equal(2025, ticketNumber.Year);
        Assert.Equal(1, ticketNumber.Sequential);
    }

    #endregion
}
