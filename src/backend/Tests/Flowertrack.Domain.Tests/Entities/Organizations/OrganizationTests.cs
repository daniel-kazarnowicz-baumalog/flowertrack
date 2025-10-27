using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Events;

namespace Flowertrack.Domain.Tests.Entities.Organizations;

public class OrganizationTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrganization()
    {
        // Arrange
        var name = "Test Organization";
        var email = "test@example.com";
        var phone = "+1234567890";

        // Act
        var organization = Organization.Create(
            name: name,
            email: email,
            phone: phone);

        // Assert
        Assert.NotEqual(Guid.Empty, organization.Id);
        Assert.Equal(name, organization.Name);
        Assert.Equal(email, organization.Email);
        Assert.Equal(phone, organization.Phone);
        Assert.Equal(ServiceStatus.Active, organization.ServiceStatus);
        Assert.Null(organization.ApiKey);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Organization.Create(name: ""));

        Assert.Contains("name cannot be empty", exception.Message);
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Organization.Create(name: "   "));

        Assert.Contains("name cannot be empty", exception.Message);
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var longName = new string('A', 256);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Organization.Create(name: longName));

        Assert.Contains("cannot exceed 255 characters", exception.Message);
    }

    [Fact]
    public void Create_ShouldRaiseOrganizationCreatedEvent()
    {
        // Arrange
        var name = "Test Organization";

        // Act
        var organization = Organization.Create(name: name);

        // Assert
        Assert.Single(organization.DomainEvents);
        var domainEvent = organization.DomainEvents.First();
        Assert.IsType<OrganizationCreatedEvent>(domainEvent);
        var createdEvent = (OrganizationCreatedEvent)domainEvent;
        Assert.Equal(organization.Id, createdEvent.OrganizationId);
        Assert.Equal(name, createdEvent.Name);
    }

    [Fact]
    public void UpdateServiceStatus_WithValidData_ShouldUpdateStatus()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        var reason = "Contract review";

        // Act
        organization.UpdateServiceStatus(ServiceStatus.Suspended, reason);

        // Assert
        Assert.Equal(ServiceStatus.Suspended, organization.ServiceStatus);
    }

    [Fact]
    public void UpdateServiceStatus_ShouldRaiseEvent()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        organization.ClearDomainEvents();
        var reason = "Contract review";

        // Act
        organization.UpdateServiceStatus(ServiceStatus.Suspended, reason);

        // Assert
        Assert.Single(organization.DomainEvents);
        var domainEvent = organization.DomainEvents.First();
        Assert.IsType<OrganizationServiceStatusChangedEvent>(domainEvent);
        var statusChangedEvent = (OrganizationServiceStatusChangedEvent)domainEvent;
        Assert.Equal(ServiceStatus.Active, statusChangedEvent.PreviousStatus);
        Assert.Equal(ServiceStatus.Suspended, statusChangedEvent.NewStatus);
        Assert.Equal(reason, statusChangedEvent.Reason);
    }

    [Fact]
    public void UpdateServiceStatus_WithSameStatus_ShouldNotRaiseEvent()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        organization.ClearDomainEvents();

        // Act
        organization.UpdateServiceStatus(ServiceStatus.Active, "Already active");

        // Assert
        Assert.Empty(organization.DomainEvents);
    }

    [Fact]
    public void UpdateServiceStatus_WithEmptyReason_ShouldThrowArgumentException()
    {
        // Arrange
        var organization = Organization.Create("Test Org");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            organization.UpdateServiceStatus(ServiceStatus.Suspended, ""));

        Assert.Contains("Reason for status change cannot be empty", exception.Message);
    }

    [Fact]
    public void SuspendService_WithValidReason_ShouldSuspendService()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        var reason = "Payment issue";

        // Act
        organization.SuspendService(reason);

        // Assert
        Assert.Equal(ServiceStatus.Suspended, organization.ServiceStatus);
    }

    [Fact]
    public void SuspendService_ShouldRaiseBothEvents()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        organization.ClearDomainEvents();
        var reason = "Payment issue";

        // Act
        organization.SuspendService(reason);

        // Assert
        Assert.Equal(2, organization.DomainEvents.Count);
        Assert.Contains(organization.DomainEvents, e => e is OrganizationServiceSuspendedEvent);
        Assert.Contains(organization.DomainEvents, e => e is OrganizationServiceStatusChangedEvent);
    }

    [Fact]
    public void SuspendService_WhenAlreadySuspended_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        organization.SuspendService("First suspension");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            organization.SuspendService("Second suspension"));

        Assert.Contains("already suspended", exception.Message);
    }

    [Fact]
    public void SuspendService_WithEmptyReason_ShouldThrowArgumentException()
    {
        // Arrange
        var organization = Organization.Create("Test Org");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            organization.SuspendService(""));

        Assert.Contains("Reason for suspension cannot be empty", exception.Message);
    }

    [Fact]
    public void ReactivateService_WhenSuspended_ShouldReactivateService()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
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
    public void ReactivateService_WhenNotSuspended_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var organization = Organization.Create("Test Org");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            organization.ReactivateService());

        Assert.Contains("Only suspended services can be reactivated", exception.Message);
    }

    [Fact]
    public void UpdateContactInfo_WithValidData_ShouldUpdateContactInfo()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        var newEmail = "new@example.com";
        var newPhone = "+9876543210";
        var newAddress = "123 New Street";

        // Act
        organization.UpdateContactInfo(newEmail, newPhone, newAddress);

        // Assert
        Assert.Equal(newEmail, organization.Email);
        Assert.Equal(newPhone, organization.Phone);
        Assert.Equal(newAddress, organization.Address);
    }

    [Fact]
    public void UpdateContactInfo_WithEmailTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        var longEmail = new string('a', 256) + "@test.com";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            organization.UpdateContactInfo(longEmail, null, null));

        Assert.Contains("Email cannot exceed 255 characters", exception.Message);
    }

    [Fact]
    public void RenewContract_WithValidDate_ShouldRenewContract()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        var newEndDate = DateTimeOffset.UtcNow.AddYears(1);

        // Act
        organization.RenewContract(newEndDate);

        // Assert
        Assert.NotNull(organization.ContractStartDate);
        Assert.Equal(newEndDate, organization.ContractEndDate);
        Assert.Equal(ServiceStatus.Active, organization.ServiceStatus);
    }

    [Fact]
    public void RenewContract_ShouldRaiseEvent()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        organization.ClearDomainEvents();
        var newEndDate = DateTimeOffset.UtcNow.AddYears(1);

        // Act
        organization.RenewContract(newEndDate);

        // Assert
        Assert.Single(organization.DomainEvents);
        var domainEvent = organization.DomainEvents.First();
        Assert.IsType<OrganizationContractRenewedEvent>(domainEvent);
        var renewedEvent = (OrganizationContractRenewedEvent)domainEvent;
        Assert.Equal(organization.Id, renewedEvent.OrganizationId);
        Assert.Equal(newEndDate, renewedEvent.NewEndDate);
    }

    [Fact]
    public void RenewContract_WithPastDate_ShouldThrowArgumentException()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        var pastDate = DateTimeOffset.UtcNow.AddDays(-1);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            organization.RenewContract(pastDate));

        Assert.Contains("Contract end date must be in the future", exception.Message);
    }

    [Fact]
    public void RenewContract_WithDateBeforeCurrentEndDate_ShouldThrowArgumentException()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        organization.RenewContract(DateTimeOffset.UtcNow.AddYears(2));
        var earlierDate = DateTimeOffset.UtcNow.AddYears(1);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            organization.RenewContract(earlierDate));

        Assert.Contains("New contract end date must be after the current end date", exception.Message);
    }

    [Fact]
    public void GenerateApiKey_ShouldGenerateUniqueKey()
    {
        // Arrange
        var organization = Organization.Create("Test Org");

        // Act
        var apiKey = organization.GenerateApiKey();

        // Assert
        Assert.NotNull(apiKey);
        Assert.NotEmpty(apiKey);
        Assert.Equal(apiKey, organization.ApiKey);
    }

    [Fact]
    public void GenerateApiKey_ShouldRaiseEvent()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        organization.ClearDomainEvents();

        // Act
        organization.GenerateApiKey();

        // Assert
        Assert.Single(organization.DomainEvents);
        var domainEvent = organization.DomainEvents.First();
        Assert.IsType<OrganizationApiKeyGeneratedEvent>(domainEvent);
        var generatedEvent = (OrganizationApiKeyGeneratedEvent)domainEvent;
        Assert.Equal(organization.Id, generatedEvent.OrganizationId);
        Assert.False(generatedEvent.IsRegeneration);
    }

    [Fact]
    public void GenerateApiKey_SecondTime_ShouldRegenerateKey()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        var firstKey = organization.GenerateApiKey();
        organization.ClearDomainEvents();

        // Act
        var secondKey = organization.GenerateApiKey();

        // Assert
        Assert.NotEqual(firstKey, secondKey);
        Assert.Single(organization.DomainEvents);
        var generatedEvent = (OrganizationApiKeyGeneratedEvent)organization.DomainEvents.First();
        Assert.True(generatedEvent.IsRegeneration);
    }

    [Fact]
    public void CanRegisterMachines_WhenActive_ShouldReturnTrue()
    {
        // Arrange
        var organization = Organization.Create("Test Org");

        // Act
        var canRegister = organization.CanRegisterMachines();

        // Assert
        Assert.True(canRegister);
    }

    [Fact]
    public void CanRegisterMachines_WhenSuspended_ShouldReturnFalse()
    {
        // Arrange
        var organization = Organization.Create("Test Org");
        organization.SuspendService("Test");

        // Act
        var canRegister = organization.CanRegisterMachines();

        // Assert
        Assert.False(canRegister);
    }

    [Fact]
    public void Create_WithAllParameters_ShouldSetAllProperties()
    {
        // Arrange
        var name = "Complete Organization";
        var email = "complete@example.com";
        var phone = "+1234567890";
        var address = "123 Main St";
        var city = "Test City";
        var postalCode = "12345";
        var country = "Test Country";
        var notes = "Test notes";

        // Act
        var organization = Organization.Create(
            name: name,
            email: email,
            phone: phone,
            address: address,
            city: city,
            postalCode: postalCode,
            country: country,
            notes: notes);

        // Assert
        Assert.Equal(name, organization.Name);
        Assert.Equal(email, organization.Email);
        Assert.Equal(phone, organization.Phone);
        Assert.Equal(address, organization.Address);
        Assert.Equal(city, organization.City);
        Assert.Equal(postalCode, organization.PostalCode);
        Assert.Equal(country, organization.Country);
        Assert.Equal(notes, organization.Notes);
    }

    [Fact]
    public void Create_TrimsWhitespace_FromAllStringProperties()
    {
        // Arrange & Act
        var organization = Organization.Create(
            name: "  Test Org  ",
            email: "  test@example.com  ",
            phone: "  +123456  ");

        // Assert
        Assert.Equal("Test Org", organization.Name);
        Assert.Equal("test@example.com", organization.Email);
        Assert.Equal("+123456", organization.Phone);
    }
}
