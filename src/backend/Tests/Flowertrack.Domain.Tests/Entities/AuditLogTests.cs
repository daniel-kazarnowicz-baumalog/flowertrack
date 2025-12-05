using Flowertrack.Domain.Entities;

namespace Flowertrack.Domain.Tests.Entities;

/// <summary>
/// Unit tests for AuditLog entity
/// </summary>
public class AuditLogTests
{
    private static Guid TestUserId => Guid.NewGuid();
    private const string TestUserEmail = "test@example.com";
    private const string TestActionType = "CREATE";
    private const string TestResourceType = "Ticket";
    private const string TestResourceId = "12345";
    private const string TestDescription = "Created a new ticket";
    private const string TestIpAddress = "192.168.1.100";
    private const string TestUserAgent = "Mozilla/5.0";

    #region Factory Method Tests

    [Fact]
    public void Create_WithValidData_ShouldCreateAuditLog()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var log = AuditLog.Create(
            userId: userId,
            userEmail: TestUserEmail,
            actionType: TestActionType,
            resourceType: TestResourceType,
            resourceId: TestResourceId,
            description: TestDescription,
            isSuccess: true);

        // Assert
        Assert.NotNull(log);
        Assert.Equal(userId, log.UserId);
        Assert.Equal(TestUserEmail, log.UserEmail);
        Assert.Equal(TestActionType, log.ActionType);
        Assert.Equal(TestResourceType, log.ResourceType);
        Assert.Equal(TestResourceId, log.ResourceId);
        Assert.Equal(TestDescription, log.Description);
        Assert.True(log.IsSuccess);
        Assert.True(log.CreatedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Create_WithMinimalData_ShouldCreateAuditLog()
    {
        // Act
        var log = AuditLog.Create(
            userId: null,
            userEmail: null,
            actionType: "SYSTEM",
            isSuccess: true);

        // Assert
        Assert.NotNull(log);
        Assert.Null(log.UserId);
        Assert.Null(log.UserEmail);
        Assert.Equal("SYSTEM", log.ActionType);
        Assert.True(log.IsSuccess);
    }

    [Fact]
    public void Create_WithOldAndNewValues_ShouldSetChangeTracking()
    {
        // Arrange
        var oldValue = """{"status": "Open"}""";
        var newValue = """{"status": "InProgress"}""";

        // Act
        var log = AuditLog.Create(
            userId: TestUserId,
            userEmail: TestUserEmail,
            actionType: "UPDATE",
            resourceType: TestResourceType,
            resourceId: TestResourceId,
            oldValue: oldValue,
            newValue: newValue,
            isSuccess: true);

        // Assert
        Assert.Equal(oldValue, log.OldValue);
        Assert.Equal(newValue, log.NewValue);
    }

    [Fact]
    public void Create_WithRequestDetails_ShouldSetHttpInformation()
    {
        // Act
        var log = AuditLog.Create(
            userId: TestUserId,
            userEmail: TestUserEmail,
            actionType: TestActionType,
            ipAddress: TestIpAddress,
            userAgent: TestUserAgent,
            httpMethod: "POST",
            requestPath: "/api/tickets",
            isSuccess: true);

        // Assert
        Assert.Equal(TestIpAddress, log.IpAddress);
        Assert.Equal(TestUserAgent, log.UserAgent);
        Assert.Equal("POST", log.HttpMethod);
        Assert.Equal("/api/tickets", log.RequestPath);
    }

    [Fact]
    public void Create_WithEmptyActionType_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            AuditLog.Create(
                userId: TestUserId,
                userEmail: TestUserEmail,
                actionType: "",
                isSuccess: true));

        Assert.Contains("Action type is required", exception.Message);
    }

    #endregion

    #region Failure Recording Tests

    [Fact]
    public void Create_WithFailure_ShouldSetErrorMessage()
    {
        // Act
        var log = AuditLog.Create(
            userId: TestUserId,
            userEmail: TestUserEmail,
            actionType: TestActionType,
            resourceType: TestResourceType,
            resourceId: TestResourceId,
            isSuccess: false,
            errorMessage: "Permission denied");

        // Assert
        Assert.False(log.IsSuccess);
        Assert.Equal("Permission denied", log.ErrorMessage);
    }

    #endregion

    #region CreateSuccess Tests

    [Fact]
    public void CreateSuccess_ShouldCreateSuccessfulAuditLog()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var log = AuditLog.CreateSuccess(
            userId: userId,
            userEmail: TestUserEmail,
            actionType: AuditActionTypes.Login,
            resourceType: "User",
            resourceId: userId.ToString(),
            description: "User logged in");

        // Assert
        Assert.Equal(userId, log.UserId);
        Assert.Equal(TestUserEmail, log.UserEmail);
        Assert.Equal(AuditActionTypes.Login, log.ActionType);
        Assert.Equal("User", log.ResourceType);
        Assert.Equal(userId.ToString(), log.ResourceId);
        Assert.True(log.IsSuccess);
    }

    [Fact]
    public void CreateSuccess_WithOldAndNewValues_ShouldSetChangeTracking()
    {
        // Arrange
        var oldValue = """{"status": "Open"}""";
        var newValue = """{"status": "Closed"}""";

        // Act
        var log = AuditLog.CreateSuccess(
            userId: TestUserId,
            userEmail: TestUserEmail,
            actionType: AuditActionTypes.Update,
            resourceType: TestResourceType,
            resourceId: TestResourceId,
            oldValue: oldValue,
            newValue: newValue);

        // Assert
        Assert.Equal(AuditActionTypes.Update, log.ActionType);
        Assert.Equal(oldValue, log.OldValue);
        Assert.Equal(newValue, log.NewValue);
        Assert.True(log.IsSuccess);
    }

    #endregion

    #region CreateFailure Tests

    [Fact]
    public void CreateFailure_ShouldCreateFailedAuditLog()
    {
        // Act
        var log = AuditLog.CreateFailure(
            userId: null,
            userEmail: TestUserEmail,
            actionType: AuditActionTypes.Login,
            errorMessage: "Invalid password",
            resourceType: "User",
            description: "Failed login attempt");

        // Assert
        Assert.Null(log.UserId);
        Assert.Equal(TestUserEmail, log.UserEmail);
        Assert.Equal(AuditActionTypes.Login, log.ActionType);
        Assert.False(log.IsSuccess);
        Assert.Equal("Invalid password", log.ErrorMessage);
    }

    [Fact]
    public void CreateFailure_ForUnauthorizedAccess_ShouldRecordDetails()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var log = AuditLog.CreateFailure(
            userId: userId,
            userEmail: TestUserEmail,
            actionType: AuditActionTypes.Read,
            errorMessage: "Access denied to resource",
            resourceType: TestResourceType,
            resourceId: TestResourceId,
            description: "User attempted unauthorized access");

        // Assert
        Assert.Equal(userId, log.UserId);
        Assert.Equal(AuditActionTypes.Read, log.ActionType);
        Assert.Equal(TestResourceType, log.ResourceType);
        Assert.Equal(TestResourceId, log.ResourceId);
        Assert.False(log.IsSuccess);
        Assert.Equal("Access denied to resource", log.ErrorMessage);
    }

    #endregion

    #region Create Tests for Different Action Types

    [Fact]
    public void Create_ForLoginAction_ShouldSetCorrectActionType()
    {
        // Act
        var log = AuditLog.Create(
            userId: TestUserId,
            userEmail: TestUserEmail,
            actionType: AuditActionTypes.Login,
            resourceType: "User",
            resourceId: TestUserId.ToString(),
            ipAddress: TestIpAddress,
            userAgent: TestUserAgent,
            isSuccess: true);

        // Assert
        Assert.Equal(AuditActionTypes.Login, log.ActionType);
        Assert.Equal(TestIpAddress, log.IpAddress);
        Assert.Equal(TestUserAgent, log.UserAgent);
        Assert.True(log.IsSuccess);
    }

    [Fact]
    public void Create_ForLogoutAction_ShouldSetCorrectActionType()
    {
        // Act
        var log = AuditLog.Create(
            userId: TestUserId,
            userEmail: TestUserEmail,
            actionType: AuditActionTypes.Logout,
            ipAddress: TestIpAddress,
            userAgent: TestUserAgent,
            isSuccess: true);

        // Assert
        Assert.Equal(AuditActionTypes.Logout, log.ActionType);
        Assert.True(log.IsSuccess);
    }

    [Fact]
    public void Create_ForCreateAction_ShouldRecordNewValue()
    {
        // Arrange
        var newValue = """{"title": "New Ticket", "priority": "High"}""";

        // Act
        var log = AuditLog.Create(
            userId: TestUserId,
            userEmail: TestUserEmail,
            actionType: AuditActionTypes.Create,
            resourceType: TestResourceType,
            resourceId: TestResourceId,
            newValue: newValue,
            ipAddress: TestIpAddress,
            userAgent: TestUserAgent,
            isSuccess: true);

        // Assert
        Assert.Equal(AuditActionTypes.Create, log.ActionType);
        Assert.Equal(TestResourceType, log.ResourceType);
        Assert.Equal(TestResourceId, log.ResourceId);
        Assert.Null(log.OldValue);
        Assert.Equal(newValue, log.NewValue);
        Assert.True(log.IsSuccess);
    }

    [Fact]
    public void Create_ForDeleteAction_ShouldRecordOldValue()
    {
        // Arrange
        var oldValue = """{"title": "Deleted Ticket"}""";

        // Act
        var log = AuditLog.Create(
            userId: TestUserId,
            userEmail: TestUserEmail,
            actionType: AuditActionTypes.Delete,
            resourceType: TestResourceType,
            resourceId: TestResourceId,
            oldValue: oldValue,
            ipAddress: TestIpAddress,
            userAgent: TestUserAgent,
            isSuccess: true);

        // Assert
        Assert.Equal(AuditActionTypes.Delete, log.ActionType);
        Assert.Equal(oldValue, log.OldValue);
        Assert.Null(log.NewValue);
        Assert.True(log.IsSuccess);
    }

    #endregion

    #region Correlation ID Tests

    [Fact]
    public void Create_WithCorrelationId_ShouldSetCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();

        // Act
        var log = AuditLog.Create(
            userId: TestUserId,
            userEmail: TestUserEmail,
            actionType: TestActionType,
            correlationId: correlationId,
            isSuccess: true);

        // Assert
        Assert.Equal(correlationId, log.CorrelationId);
    }

    #endregion

    #region AuditActionTypes Constants Tests

    [Fact]
    public void AuditActionTypes_ShouldHaveExpectedValues()
    {
        // Assert - common action types
        Assert.Equal("CREATE", AuditActionTypes.Create);
        Assert.Equal("UPDATE", AuditActionTypes.Update);
        Assert.Equal("DELETE", AuditActionTypes.Delete);
        Assert.Equal("LOGIN", AuditActionTypes.Login);
        Assert.Equal("LOGOUT", AuditActionTypes.Logout);
        Assert.Equal("READ", AuditActionTypes.Read);
        Assert.Equal("EXPORT", AuditActionTypes.Export);
    }

    #endregion
}
