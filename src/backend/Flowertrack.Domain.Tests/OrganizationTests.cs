using Flowertrack.Api.Domain.Entities;
using Flowertrack.Api.Domain.Enums;
using Flowertrack.Api.Domain.Events;

namespace Flowertrack.Domain.Tests;

/// <summary>
/// Unit tests for the Organization entity
/// </summary>
public class OrganizationTests
{
    #region Factory Method Tests

    [Fact]
    public void Create_WithValidData_ShouldCreateOrganization()
    {
        // Arrange
        var name = "Test Organization";
        var email = "test@example.com";
        var phone = "+48123456789";

        // Act
        var organization = Organization.Create(
            name: name,
            email: email,
            phone: phone);

        // Assert
        Assert.NotNull(organization);
        Assert.NotEqual(Guid.Empty, organization.Id);
        Assert.Equal(name, organization.Name);
        Assert.Equal(email.ToLowerInvariant(), organization.Email.Value);
        Assert.Equal(phone, organization.Phone);
        Assert.Equal(ServiceStatus.Active, organization.ServiceStatus);
        Assert.Single(organization.DomainEvents);
        Assert.IsType<OrganizationCreatedEvent>(organization.DomainEvents.First());
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Organization.Create(
                name: "",
                email: "test@example.com"));

        Assert.Contains("name cannot be empty", exception.Message);
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var longName = new string('a', 256);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Organization.Create(
                name: longName,
                email: "test@example.com"));

        Assert.Contains("cannot exceed 255 characters", exception.Message);
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Organization.Create(
                name: "Test Org",
                email: "invalid-email"));
    }

    [Fact]
    public void Create_WithInvalidContractDates_ShouldThrowArgumentException()
    {
        // Arrange
        var startDate = DateTimeOffset.UtcNow;
        var endDate = startDate.AddDays(-1);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Organization.Create(
                name: "Test Org",
                email: "test@example.com",
                contractStartDate: startDate,
                contractEndDate: endDate));

        Assert.Contains("end date must be after start date", exception.Message);
    }

    #endregion

    #region UpdateServiceStatus Tests

    [Fact]
    public void UpdateServiceStatus_WithValidData_ShouldUpdateStatus()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");
        organization.ClearDomainEvents();

        // Act
        organization.UpdateServiceStatus(ServiceStatus.Suspended, "Testing suspension");

        // Assert
        Assert.Equal(ServiceStatus.Suspended, organization.ServiceStatus);
        Assert.Single(organization.DomainEvents);
        
        var domainEvent = organization.DomainEvents.First() as OrganizationServiceStatusChangedEvent;
        Assert.NotNull(domainEvent);
        Assert.Equal(ServiceStatus.Active, domainEvent.PreviousStatus);
        Assert.Equal(ServiceStatus.Suspended, domainEvent.NewStatus);
        Assert.Equal("Testing suspension", domainEvent.Reason);
    }

    [Fact]
    public void UpdateServiceStatus_WithEmptyReason_ShouldThrowArgumentException()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            organization.UpdateServiceStatus(ServiceStatus.Suspended, ""));

        Assert.Contains("Reason for status change cannot be empty", exception.Message);
    }

    [Fact]
    public void UpdateServiceStatus_WithSameStatus_ShouldNotRaiseEvent()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");
        organization.ClearDomainEvents();

        // Act
        organization.UpdateServiceStatus(ServiceStatus.Active, "No change");

        // Assert
        Assert.Equal(ServiceStatus.Active, organization.ServiceStatus);
        Assert.Empty(organization.DomainEvents);
    }

    #endregion

    #region SuspendService Tests

    [Fact]
    public void SuspendService_WithValidReason_ShouldSuspendService()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");
        organization.ClearDomainEvents();

        // Act
        organization.SuspendService("Payment overdue");

        // Assert
        Assert.Equal(ServiceStatus.Suspended, organization.ServiceStatus);
        Assert.Equal(2, organization.DomainEvents.Count);
        Assert.Contains(organization.DomainEvents, e => e is OrganizationServiceSuspendedEvent);
        Assert.Contains(organization.DomainEvents, e => e is OrganizationServiceStatusChangedEvent);
    }

    [Fact]
    public void SuspendService_WhenAlreadySuspended_ShouldNotRaiseEvents()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");
        organization.SuspendService("First suspension");
        organization.ClearDomainEvents();

        // Act
        organization.SuspendService("Second suspension attempt");

        // Assert
        Assert.Equal(ServiceStatus.Suspended, organization.ServiceStatus);
        Assert.Empty(organization.DomainEvents);
    }

    [Fact]
    public void SuspendService_WithEmptyReason_ShouldThrowArgumentException()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            organization.SuspendService(""));

        Assert.Contains("Reason for suspension cannot be empty", exception.Message);
    }

    #endregion

    #region ReactivateService Tests

    [Fact]
    public void ReactivateService_FromSuspended_ShouldActivateService()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");
        organization.SuspendService("Test suspension");
        organization.ClearDomainEvents();

        // Act
        organization.ReactivateService();

        // Assert
        Assert.Equal(ServiceStatus.Active, organization.ServiceStatus);
        Assert.Single(organization.DomainEvents);
        Assert.IsType<OrganizationServiceStatusChangedEvent>(organization.DomainEvents.First());
    }

    [Fact]
    public void ReactivateService_WhenAlreadyActive_ShouldNotRaiseEvents()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");
        organization.ClearDomainEvents();

        // Act
        organization.ReactivateService();

        // Assert
        Assert.Equal(ServiceStatus.Active, organization.ServiceStatus);
        Assert.Empty(organization.DomainEvents);
    }

    [Fact]
    public void ReactivateService_WithExpiredContract_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com",
            contractStartDate: DateTimeOffset.UtcNow.AddDays(-30),
            contractEndDate: DateTimeOffset.UtcNow.AddDays(-1));
        organization.SuspendService("Test suspension");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            organization.ReactivateService());

        Assert.Contains("expired contract", exception.Message);
    }

    #endregion

    #region UpdateContactInfo Tests

    [Fact]
    public void UpdateContactInfo_WithValidData_ShouldUpdateContactInfo()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "old@example.com");

        // Act
        organization.UpdateContactInfo(
            email: "new@example.com",
            phone: "+48987654321",
            address: "New Address 123");

        // Assert
        Assert.Equal("new@example.com", organization.Email.Value);
        Assert.Equal("+48987654321", organization.Phone);
        Assert.Equal("New Address 123", organization.Address);
    }

    [Fact]
    public void UpdateContactInfo_WithInvalidEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            organization.UpdateContactInfo(
                email: "invalid-email",
                phone: "+48123456789",
                address: "Address"));
    }

    #endregion

    #region RenewContract Tests

    [Fact]
    public void RenewContract_WithValidDate_ShouldRenewContract()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com",
            contractStartDate: DateTimeOffset.UtcNow.AddDays(-30),
            contractEndDate: DateTimeOffset.UtcNow.AddDays(30));
        organization.ClearDomainEvents();

        var newEndDate = DateTimeOffset.UtcNow.AddDays(365);

        // Act
        organization.RenewContract(newEndDate);

        // Assert
        Assert.Equal(newEndDate, organization.ContractEndDate);
        Assert.Single(organization.DomainEvents);
        Assert.IsType<OrganizationContractRenewedEvent>(organization.DomainEvents.First());
    }

    [Fact]
    public void RenewContract_WithPastDate_ShouldThrowArgumentException()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            organization.RenewContract(DateTimeOffset.UtcNow.AddDays(-1)));

        Assert.Contains("must be in the future", exception.Message);
    }

    [Fact]
    public void RenewContract_WhenExpired_ShouldReactivateService()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com",
            serviceStatus: ServiceStatus.Expired,
            contractStartDate: DateTimeOffset.UtcNow.AddDays(-60),
            contractEndDate: DateTimeOffset.UtcNow.AddDays(-1));
        organization.ClearDomainEvents();

        // Act
        organization.RenewContract(DateTimeOffset.UtcNow.AddDays(365));

        // Assert
        Assert.Equal(ServiceStatus.Active, organization.ServiceStatus);
        Assert.Equal(2, organization.DomainEvents.Count);
        Assert.Contains(organization.DomainEvents, e => e is OrganizationServiceStatusChangedEvent);
        Assert.Contains(organization.DomainEvents, e => e is OrganizationContractRenewedEvent);
    }

    #endregion

    #region GenerateApiKey Tests

    [Fact]
    public void GenerateApiKey_FirstTime_ShouldGenerateKey()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");
        organization.ClearDomainEvents();

        // Act
        var apiKey = organization.GenerateApiKey();

        // Assert
        Assert.NotNull(apiKey);
        Assert.StartsWith("ft_", apiKey);
        Assert.Single(organization.DomainEvents);
        
        var domainEvent = organization.DomainEvents.First() as OrganizationApiKeyGeneratedEvent;
        Assert.NotNull(domainEvent);
        Assert.False(domainEvent.IsRegeneration);
    }

    [Fact]
    public void GenerateApiKey_SecondTime_ShouldRegenerateKey()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");
        var firstKey = organization.GenerateApiKey();
        organization.ClearDomainEvents();

        // Act
        var secondKey = organization.GenerateApiKey();

        // Assert
        Assert.NotEqual(firstKey, secondKey);
        Assert.Single(organization.DomainEvents);
        
        var domainEvent = organization.DomainEvents.First() as OrganizationApiKeyGeneratedEvent;
        Assert.NotNull(domainEvent);
        Assert.True(domainEvent.IsRegeneration);
    }

    #endregion

    #region Business Rules Tests

    [Fact]
    public void CanRegisterMachines_WhenActive_ShouldReturnTrue()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com",
            serviceStatus: ServiceStatus.Active);

        // Act
        var canRegister = organization.CanRegisterMachines();

        // Assert
        Assert.True(canRegister);
    }

    [Fact]
    public void CanRegisterMachines_WhenSuspended_ShouldReturnFalse()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");
        organization.SuspendService("Test suspension");

        // Act
        var canRegister = organization.CanRegisterMachines();

        // Assert
        Assert.False(canRegister);
    }

    [Fact]
    public void CanRegisterMachines_WhenInactive_ShouldReturnFalse()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com",
            serviceStatus: ServiceStatus.Inactive);

        // Act
        var canRegister = organization.CanRegisterMachines();

        // Assert
        Assert.False(canRegister);
    }

    [Fact]
    public void CheckAndUpdateContractStatus_WithExpiredContract_ShouldUpdateStatus()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com",
            serviceStatus: ServiceStatus.Active,
            contractStartDate: DateTimeOffset.UtcNow.AddDays(-60),
            contractEndDate: DateTimeOffset.UtcNow.AddDays(-1));
        organization.ClearDomainEvents();

        // Act
        organization.CheckAndUpdateContractStatus();

        // Assert
        Assert.Equal(ServiceStatus.Expired, organization.ServiceStatus);
        Assert.Single(organization.DomainEvents);
        Assert.IsType<OrganizationServiceStatusChangedEvent>(organization.DomainEvents.First());
    }

    [Fact]
    public void CheckAndUpdateContractStatus_WithValidContract_ShouldNotUpdateStatus()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com",
            serviceStatus: ServiceStatus.Active,
            contractStartDate: DateTimeOffset.UtcNow.AddDays(-30),
            contractEndDate: DateTimeOffset.UtcNow.AddDays(30));
        organization.ClearDomainEvents();

        // Act
        organization.CheckAndUpdateContractStatus();

        // Assert
        Assert.Equal(ServiceStatus.Active, organization.ServiceStatus);
        Assert.Empty(organization.DomainEvents);
    }

    #endregion

    #region Audit Tests

    [Fact]
    public void Create_ShouldSetCreatedAuditProperties()
    {
        // Arrange & Act
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com",
            createdBy: "test-user");

        // Assert
        Assert.NotEqual(default, organization.CreatedAt);
        Assert.Equal("test-user", organization.CreatedBy);
        Assert.Null(organization.UpdatedAt);
        Assert.Null(organization.UpdatedBy);
    }

    [Fact]
    public void UpdateServiceStatus_ShouldSetUpdatedAuditProperties()
    {
        // Arrange
        var organization = Organization.Create(
            name: "Test Org",
            email: "test@example.com");

        // Act
        organization.UpdateServiceStatus(ServiceStatus.Suspended, "Test", "update-user");

        // Assert
        Assert.NotNull(organization.UpdatedAt);
        Assert.Equal("update-user", organization.UpdatedBy);
    }

    #endregion
}
