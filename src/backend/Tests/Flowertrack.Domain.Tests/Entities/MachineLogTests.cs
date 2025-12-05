using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Events;

namespace Flowertrack.Domain.Tests.Entities;

/// <summary>
/// Unit tests for MachineLog entity
/// </summary>
public class MachineLogTests
{
    private static Guid TestMachineId => Guid.NewGuid();
    private const string TestLogContent = """{"temperature": 25.5, "pressure": 1.01}""";
    private const string TestLogType = "TELEMETRY";
    private const string TestStatus = "NORMAL";
    private const string TestAlarmCode = "ALM001";
    private const string TestSeverity = "WARNING";

    #region Factory Method Tests

    [Fact]
    public void Create_WithValidData_ShouldCreateMachineLog()
    {
        // Arrange
        var machineId = TestMachineId;
        var logContent = TestLogContent;
        var logType = TestLogType;

        // Act
        var log = MachineLog.Create(machineId, logContent, logType);

        // Assert
        Assert.NotNull(log);
        Assert.NotEqual(Guid.Empty, log.Id);
        Assert.Equal(machineId, log.MachineId);
        Assert.Equal(logContent, log.LogContent);
        Assert.Equal(logType, log.LogType);
        Assert.Equal("INFO", log.Severity);
        Assert.Null(log.Status);
        Assert.Null(log.AlarmCode);
        Assert.False(log.IsProcessed);
        Assert.True(log.ReceivedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Create_WithAllParameters_ShouldSetAllProperties()
    {
        // Arrange
        var machineId = TestMachineId;
        var machineTimestamp = DateTimeOffset.UtcNow.AddMinutes(-5);

        // Act
        var log = MachineLog.Create(
            machineId: machineId,
            logContent: TestLogContent,
            logType: TestLogType,
            status: TestStatus,
            alarmCode: TestAlarmCode,
            alarmMessage: "Temperature warning",
            severity: TestSeverity,
            machineTimestamp: machineTimestamp);

        // Assert
        Assert.Equal(machineId, log.MachineId);
        Assert.Equal(TestLogContent, log.LogContent);
        Assert.Equal(TestLogType, log.LogType);
        Assert.Equal(TestStatus, log.Status);
        Assert.Equal(TestAlarmCode, log.AlarmCode);
        Assert.Equal("Temperature warning", log.AlarmMessage);
        Assert.Equal(TestSeverity, log.Severity);
        Assert.Equal(machineTimestamp, log.MachineTimestamp);
    }

    [Fact]
    public void Create_ShouldRaiseMachineLogReceivedEvent()
    {
        // Arrange & Act
        var machineId = TestMachineId;
        var log = MachineLog.Create(machineId, TestLogContent, TestLogType);

        // Assert
        var domainEvents = log.DomainEvents.ToList();
        Assert.Single(domainEvents);
        var receivedEvent = Assert.IsType<MachineLogReceivedEvent>(domainEvents[0]);
        Assert.Equal(log.Id, receivedEvent.LogId);
        Assert.Equal(machineId, receivedEvent.MachineId);
        Assert.Equal(TestLogType, receivedEvent.LogType);
    }

    [Fact]
    public void Create_WithEmptyMachineId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            MachineLog.Create(Guid.Empty, TestLogContent, TestLogType));

        Assert.Contains("Machine ID is required", exception.Message);
    }

    [Fact]
    public void Create_WithEmptyLogContent_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            MachineLog.Create(TestMachineId, "", TestLogType));

        Assert.Contains("Log content cannot be empty", exception.Message);
    }

    [Fact]
    public void Create_WithEmptyLogType_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            MachineLog.Create(TestMachineId, TestLogContent, ""));

        Assert.Contains("Log type is required", exception.Message);
    }

    #endregion

    #region MarkAsProcessed Tests

    [Fact]
    public void MarkAsProcessed_ShouldSetIsProcessedToTrue()
    {
        // Arrange
        var log = MachineLog.Create(TestMachineId, TestLogContent, TestLogType);
        Assert.False(log.IsProcessed);

        // Act
        log.MarkAsProcessed();

        // Assert
        Assert.True(log.IsProcessed);
    }

    [Fact]
    public void MarkAsProcessed_WhenCalledMultipleTimes_ShouldRemainProcessed()
    {
        // Arrange
        var log = MachineLog.Create(TestMachineId, TestLogContent, TestLogType);
        log.MarkAsProcessed();

        // Act
        log.MarkAsProcessed();

        // Assert
        Assert.True(log.IsProcessed);
    }

    #endregion

    #region Log Type Tests

    [Theory]
    [InlineData("TELEMETRY")]
    [InlineData("STATUS")]
    [InlineData("ALARM")]
    [InlineData("WARNING")]
    [InlineData("INFO")]
    public void Create_WithDifferentLogTypes_ShouldCreateSuccessfully(string logType)
    {
        // Act
        var log = MachineLog.Create(TestMachineId, TestLogContent, logType);

        // Assert
        Assert.Equal(logType, log.LogType);
    }

    #endregion

    #region Severity Tests

    [Theory]
    [InlineData("INFO")]
    [InlineData("WARNING")]
    [InlineData("ERROR")]
    [InlineData("CRITICAL")]
    public void Create_WithDifferentSeverities_ShouldSetCorrectly(string severity)
    {
        // Act
        var log = MachineLog.Create(
            TestMachineId,
            TestLogContent,
            TestLogType,
            severity: severity);

        // Assert
        Assert.Equal(severity, log.Severity);
    }

    #endregion

    #region Alarm Tests

    [Fact]
    public void Create_AlarmLog_ShouldHaveAlarmProperties()
    {
        // Act
        var log = MachineLog.Create(
            TestMachineId,
            TestLogContent,
            "ALARM",
            status: "ALARM",
            alarmCode: "ALM001",
            alarmMessage: "High temperature detected",
            severity: "CRITICAL");

        // Assert
        Assert.Equal("ALARM", log.LogType);
        Assert.Equal("ALARM", log.Status);
        Assert.Equal("ALM001", log.AlarmCode);
        Assert.Equal("High temperature detected", log.AlarmMessage);
        Assert.Equal("CRITICAL", log.Severity);
    }

    #endregion
}
