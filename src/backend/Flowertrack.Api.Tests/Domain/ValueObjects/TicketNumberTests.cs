using Flowertrack.Api.Domain.ValueObjects;

namespace Flowertrack.Api.Tests.Domain.ValueObjects;

public class TicketNumberTests
{
    [Fact]
    public void Create_WithValidYearAndSequential_ShouldSucceed()
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
    public void Create_WithMaxSequential_ShouldSucceed()
    {
        // Arrange
        var year = 2025;
        var sequential = 99999;

        // Act
        var ticketNumber = TicketNumber.Create(year, sequential);

        // Assert
        Assert.Equal("TICK-2025-99999", ticketNumber.Value);
    }

    [Fact]
    public void Create_WithMinYear_ShouldSucceed()
    {
        // Arrange
        var year = 2020;
        var sequential = 1;

        // Act
        var ticketNumber = TicketNumber.Create(year, sequential);

        // Assert
        Assert.Equal("TICK-2020-00001", ticketNumber.Value);
    }

    [Fact]
    public void Create_WithFutureYear_ShouldSucceed()
    {
        // Arrange
        var year = DateTime.UtcNow.Year + 1;
        var sequential = 1;

        // Act
        var ticketNumber = TicketNumber.Create(year, sequential);

        // Assert
        Assert.Equal(year, ticketNumber.Year);
    }

    [Theory]
    [InlineData(2019)] // Below minimum
    [InlineData(1999)]
    public void Create_WithYearBelowMinimum_ShouldThrowArgumentException(int year)
    {
        // Arrange
        var sequential = 1;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => TicketNumber.Create(year, sequential));
        Assert.Contains("Year must be between", exception.Message);
    }

    [Fact]
    public void Create_WithYearTooFarInFuture_ShouldThrowArgumentException()
    {
        // Arrange
        var year = DateTime.UtcNow.Year + 2;
        var sequential = 1;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => TicketNumber.Create(year, sequential));
        Assert.Contains("Year must be between", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100000)]
    [InlineData(100001)]
    public void Create_WithInvalidSequential_ShouldThrowArgumentException(int sequential)
    {
        // Arrange
        var year = 2025;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => TicketNumber.Create(year, sequential));
        Assert.Contains("Sequential must be between", exception.Message);
    }

    [Theory]
    [InlineData("TICK-2025-00001")]
    [InlineData("TICK-2020-00001")]
    [InlineData("TICK-2025-99999")]
    public void Parse_WithValidFormat_ShouldSucceed(string value)
    {
        // Act
        var ticketNumber = TicketNumber.Parse(value);

        // Assert
        Assert.NotNull(ticketNumber);
        Assert.Equal(value, ticketNumber.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_WithNullOrEmpty_ShouldThrowArgumentException(string? value)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => TicketNumber.Parse(value!));
        Assert.Contains("cannot be null or empty", exception.Message);
    }

    [Theory]
    [InlineData("TICK-2025-0001")] // Too few digits in sequential
    [InlineData("TICK-25-00001")] // Too few digits in year
    [InlineData("TICK-2025-1")] // Missing leading zeros
    [InlineData("TIC-2025-00001")] // Wrong prefix
    [InlineData("TICKET-2025-00001")] // Wrong prefix
    [InlineData("2025-00001")] // Missing prefix
    [InlineData("TICK_2025_00001")] // Wrong separator
    [InlineData("tick-2025-00001")] // Lowercase
    public void Parse_WithInvalidFormat_ShouldThrowArgumentException(string value)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => TicketNumber.Parse(value));
        Assert.Contains("Invalid ticket number format", exception.Message);
    }

    [Theory]
    [InlineData("TICK-2025-00001", true)]
    [InlineData("TICK-2020-00001", true)]
    [InlineData("TICK-2025-99999", true)]
    public void TryParse_WithValidFormat_ShouldReturnTrue(string value, bool expectedResult)
    {
        // Act
        var result = TicketNumber.TryParse(value, out var ticketNumber);

        // Assert
        Assert.Equal(expectedResult, result);
        Assert.NotNull(ticketNumber);
        Assert.Equal(value, ticketNumber?.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("TICK-2025-0001")]
    [InlineData("invalid")]
    public void TryParse_WithInvalidFormat_ShouldReturnFalse(string? value)
    {
        // Act
        var result = TicketNumber.TryParse(value, out var ticketNumber);

        // Assert
        Assert.False(result);
        Assert.Null(ticketNumber);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedValue()
    {
        // Arrange
        var ticketNumber = TicketNumber.Create(2025, 1);

        // Act
        var result = ticketNumber.ToString();

        // Assert
        Assert.Equal("TICK-2025-00001", result);
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var ticket1 = TicketNumber.Create(2025, 1);
        var ticket2 = TicketNumber.Create(2025, 1);

        // Assert
        Assert.Equal(ticket1, ticket2);
        Assert.True(ticket1 == ticket2);
        Assert.False(ticket1 != ticket2);
        Assert.Equal(ticket1.GetHashCode(), ticket2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var ticket1 = TicketNumber.Create(2025, 1);
        var ticket2 = TicketNumber.Create(2025, 2);

        // Assert
        Assert.NotEqual(ticket1, ticket2);
        Assert.False(ticket1 == ticket2);
        Assert.True(ticket1 != ticket2);
    }

    [Fact]
    public void Equality_WithDifferentYears_ShouldNotBeEqual()
    {
        // Arrange
        var ticket1 = TicketNumber.Create(2024, 1);
        var ticket2 = TicketNumber.Create(2025, 1);

        // Assert
        Assert.NotEqual(ticket1, ticket2);
    }

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
        Assert.Equal(value, ticketNumber.Value);
    }
}
