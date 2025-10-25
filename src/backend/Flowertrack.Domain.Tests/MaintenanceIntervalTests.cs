using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Tests;

public class MaintenanceIntervalTests
{
    [Fact]
    public void Create_WithValidParameters_CreatesMaintenanceInterval()
    {
        // Arrange
        var id = 1;
        var days = 90;
        var description = "Quarterly Maintenance";

        // Act
        var interval = MaintenanceInterval.Create(id, days, description);

        // Assert
        Assert.NotNull(interval);
        Assert.Equal(id, interval.Id);
        Assert.Equal(days, interval.Days);
        Assert.Equal(description, interval.Description);
    }

    [Fact]
    public void Create_WithZeroDays_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            MaintenanceInterval.Create(1, 0, "Test"));

        Assert.Contains("Maintenance interval must be greater than zero", exception.Message);
    }

    [Fact]
    public void Create_WithNegativeDays_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            MaintenanceInterval.Create(1, -10, "Test"));

        Assert.Contains("Maintenance interval must be greater than zero", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidDescription_ThrowsArgumentException(string description)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            MaintenanceInterval.Create(1, 30, description));

        Assert.Contains("Description is required", exception.Message);
    }

    [Fact]
    public void CalculateNextDate_ReturnsCorrectDate()
    {
        // Arrange
        var interval = MaintenanceInterval.Create(1, 90, "Quarterly");
        var fromDate = new DateOnly(2024, 1, 1);

        // Act
        var nextDate = interval.CalculateNextDate(fromDate);

        // Assert
        Assert.Equal(new DateOnly(2024, 3, 31), nextDate);
    }

    [Fact]
    public void CalculateNextDate_WithDifferentDays_ReturnsCorrectDate()
    {
        // Arrange
        var interval = MaintenanceInterval.Create(1, 30, "Monthly");
        var fromDate = new DateOnly(2024, 1, 15);

        // Act
        var nextDate = interval.CalculateNextDate(fromDate);

        // Assert
        Assert.Equal(new DateOnly(2024, 2, 14), nextDate);
    }
}
