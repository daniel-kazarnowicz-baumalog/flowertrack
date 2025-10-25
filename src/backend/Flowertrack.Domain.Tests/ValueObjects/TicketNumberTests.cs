using FluentAssertions;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Tests.ValueObjects;

public class TicketNumberTests
{
    [Theory]
    [InlineData(2025, 1, "TICK-2025-00001")]
    [InlineData(2025, 42, "TICK-2025-00042")]
    [InlineData(2025, 999, "TICK-2025-00999")]
    [InlineData(2025, 12345, "TICK-2025-12345")]
    [InlineData(2025, 99999, "TICK-2025-99999")]
    public void Create_WithValidYearAndSequence_ShouldCreateCorrectFormat(int year, int sequence, string expectedValue)
    {
        // Act
        var ticketNumber = TicketNumber.Create(year, sequence);

        // Assert
        ticketNumber.Value.Should().Be(expectedValue);
        ticketNumber.ToString().Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(1999)]
    [InlineData(10000)]
    public void Create_WithInvalidYear_ShouldThrowException(int year)
    {
        // Act
        Action act = () => TicketNumber.Create(year, 1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Year must be between 2000 and 9999*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100000)]
    public void Create_WithInvalidSequence_ShouldThrowException(int sequence)
    {
        // Act
        Action act = () => TicketNumber.Create(2025, sequence);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Sequence must be between 1 and 99999*");
    }

    [Theory]
    [InlineData("TICK-2025-00001")]
    [InlineData("TICK-2025-12345")]
    [InlineData("TICK-2025-99999")]
    public void Parse_WithValidFormat_ShouldParseCorrectly(string value)
    {
        // Act
        var ticketNumber = TicketNumber.Parse(value);

        // Assert
        ticketNumber.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Parse_WithNullOrEmpty_ShouldThrowException(string? value)
    {
        // Act
        Action act = () => TicketNumber.Parse(value!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Ticket number cannot be null or empty*");
    }

    [Theory]
    [InlineData("INVALID")]
    [InlineData("TICK-2025")]
    [InlineData("TICK-2025-00001-EXTRA")]
    public void Parse_WithInvalidFormat_ShouldThrowException(string value)
    {
        // Act
        Action act = () => TicketNumber.Parse(value);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid ticket number format*");
    }

    [Fact]
    public void Parse_WithInvalidPrefix_ShouldThrowException()
    {
        // Act
        Action act = () => TicketNumber.Parse("WRONG-2025-00001");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid ticket number prefix*");
    }

    [Theory]
    [InlineData("TICK-1999-00001")]
    [InlineData("TICK-10000-00001")]
    [InlineData("TICK-ABCD-00001")]
    public void Parse_WithInvalidYear_ShouldThrowException(string value)
    {
        // Act
        Action act = () => TicketNumber.Parse(value);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid year in ticket number*");
    }

    [Theory]
    [InlineData("TICK-2025-0001")]  // Too short
    [InlineData("TICK-2025-000001")] // Too long
    [InlineData("TICK-2025-ABCDE")]  // Not a number
    public void Parse_WithInvalidSequence_ShouldThrowException(string value)
    {
        // Act
        Action act = () => TicketNumber.Parse(value);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid sequence in ticket number*");
    }

    [Theory]
    [InlineData("TICK-2025-00001", true)]
    [InlineData("INVALID", false)]
    [InlineData("", false)]
    public void TryParse_ShouldReturnExpectedResult(string value, bool expectedSuccess)
    {
        // Act
        var success = TicketNumber.TryParse(value, out var ticketNumber);

        // Assert
        success.Should().Be(expectedSuccess);
        if (expectedSuccess)
        {
            ticketNumber.Should().NotBeNull();
            ticketNumber!.Value.Should().Be(value);
        }
        else
        {
            ticketNumber.Should().BeNull();
        }
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var ticketNumber1 = TicketNumber.Create(2025, 1);
        var ticketNumber2 = TicketNumber.Create(2025, 1);

        // Assert
        ticketNumber1.Should().Be(ticketNumber2);
        ticketNumber1.Equals(ticketNumber2).Should().BeTrue();
        (ticketNumber1 == ticketNumber2).Should().BeTrue();
        (ticketNumber1 != ticketNumber2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var ticketNumber1 = TicketNumber.Create(2025, 1);
        var ticketNumber2 = TicketNumber.Create(2025, 2);

        // Assert
        ticketNumber1.Should().NotBe(ticketNumber2);
        ticketNumber1.Equals(ticketNumber2).Should().BeFalse();
        (ticketNumber1 == ticketNumber2).Should().BeFalse();
        (ticketNumber1 != ticketNumber2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_WithSameValue_ShouldReturnSameHashCode()
    {
        // Arrange
        var ticketNumber1 = TicketNumber.Create(2025, 1);
        var ticketNumber2 = TicketNumber.Create(2025, 1);

        // Assert
        ticketNumber1.GetHashCode().Should().Be(ticketNumber2.GetHashCode());
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var ticketNumber = TicketNumber.Create(2025, 42);

        // Act
        string value = ticketNumber;

        // Assert
        value.Should().Be("TICK-2025-00042");
    }
}
