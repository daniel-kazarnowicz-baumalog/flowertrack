using Flowertrack.Api.Domain.Entities;
using Flowertrack.Api.Domain.Enums;
using Flowertrack.Api.Domain.Events;
using Flowertrack.Api.Exceptions;

namespace Flowertrack.Domain.Tests;

public class ServiceUserTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateServiceUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phoneNumber = "+1234567890";
        var specialization = "Electrical Systems";
        var createdBy = Guid.NewGuid();

        // Act
        var serviceUser = ServiceUser.Create(
            userId,
            firstName,
            lastName,
            email,
            phoneNumber,
            specialization,
            createdBy);

        // Assert
        Assert.NotNull(serviceUser);
        Assert.Equal(userId, serviceUser.Id);
        Assert.Equal(userId, serviceUser.UserId);
        Assert.Equal(firstName, serviceUser.FirstName);
        Assert.Equal(lastName, serviceUser.LastName);
        Assert.Equal(email.ToLowerInvariant(), serviceUser.Email);
        Assert.Equal(phoneNumber, serviceUser.PhoneNumber);
        Assert.Equal(specialization, serviceUser.Specialization);
        Assert.Equal(UserStatus.Pending, serviceUser.Status);
        Assert.False(serviceUser.IsAvailable);
        Assert.True(serviceUser.CreatedAt > DateTime.MinValue);
        Assert.Equal(createdBy, serviceUser.CreatedBy);
    }

    [Fact]
    public void Create_WithValidDataWithoutOptionalFields_ShouldCreateServiceUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var firstName = "Jane";
        var lastName = "Smith";
        var email = "jane.smith@example.com";

        // Act
        var serviceUser = ServiceUser.Create(userId, firstName, lastName, email);

        // Assert
        Assert.NotNull(serviceUser);
        Assert.Equal(userId, serviceUser.UserId);
        Assert.Null(serviceUser.PhoneNumber);
        Assert.Null(serviceUser.Specialization);
        Assert.Equal(UserStatus.Pending, serviceUser.Status);
    }

    [Fact]
    public void Create_ShouldRaiseDomainEvent()
    {
        // Arrange & Act
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");

        // Assert
        Assert.Single(serviceUser.DomainEvents);
        var domainEvent = serviceUser.DomainEvents.First();
        Assert.IsType<ServiceUserCreatedEvent>(domainEvent);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidFirstName_ShouldThrowValidationException(string? firstName)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            ServiceUser.Create(Guid.NewGuid(), firstName!, "Doe", "john.doe@example.com"));

        Assert.Contains("FirstName", exception.Errors.Keys);
    }

    [Fact]
    public void Create_WithFirstNameTooLong_ShouldThrowValidationException()
    {
        // Arrange
        var longFirstName = new string('A', 101);

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            ServiceUser.Create(Guid.NewGuid(), longFirstName, "Doe", "john.doe@example.com"));

        Assert.Contains("FirstName", exception.Errors.Keys);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidLastName_ShouldThrowValidationException(string? lastName)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            ServiceUser.Create(Guid.NewGuid(), "John", lastName!, "john.doe@example.com"));

        Assert.Contains("LastName", exception.Errors.Keys);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidEmail_ShouldThrowValidationException(string? email)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            ServiceUser.Create(Guid.NewGuid(), "John", "Doe", email!));

        Assert.Contains("Email", exception.Errors.Keys);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public void Create_WithInvalidEmailFormat_ShouldThrowValidationException(string email)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            ServiceUser.Create(Guid.NewGuid(), "John", "Doe", email));

        Assert.Contains("Email", exception.Errors.Keys);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowValidationException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            ServiceUser.Create(Guid.Empty, "John", "Doe", "john.doe@example.com"));

        Assert.Contains("UserId", exception.Errors.Keys);
    }

    [Fact]
    public void Activate_WhenPending_ShouldActivateUser()
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.ClearDomainEvents();

        // Act
        serviceUser.Activate();

        // Assert
        Assert.Equal(UserStatus.Active, serviceUser.Status);
        Assert.True(serviceUser.IsAvailable);
        Assert.Null(serviceUser.DeactivationReason);
        Assert.Single(serviceUser.DomainEvents);
        Assert.IsType<ServiceUserActivatedEvent>(serviceUser.DomainEvents.First());
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldThrowDomainException()
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.Activate();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => serviceUser.Activate());
        Assert.Contains("already active", exception.Message);
    }

    [Fact]
    public void Deactivate_WithValidReason_ShouldDeactivateUser()
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.Activate();
        serviceUser.ClearDomainEvents();
        var reason = "Left the company";

        // Act
        serviceUser.Deactivate(reason);

        // Assert
        Assert.Equal(UserStatus.Deactivated, serviceUser.Status);
        Assert.False(serviceUser.IsAvailable);
        Assert.Equal(reason, serviceUser.DeactivationReason);
        Assert.Single(serviceUser.DomainEvents);
        Assert.IsType<ServiceUserDeactivatedEvent>(serviceUser.DomainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Deactivate_WithInvalidReason_ShouldThrowValidationException(string? reason)
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.Activate();

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            serviceUser.Deactivate(reason!));

        Assert.Contains("Reason", exception.Errors.Keys);
    }

    [Fact]
    public void Deactivate_WhenAlreadyDeactivated_ShouldThrowDomainException()
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.Activate();
        serviceUser.Deactivate("First reason");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            serviceUser.Deactivate("Second reason"));
        Assert.Contains("already deactivated", exception.Message);
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldUpdateProfile()
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.Activate();

        var newFirstName = "Jane";
        var newLastName = "Smith";
        var newPhone = "+9876543210";

        // Act
        serviceUser.UpdateProfile(newFirstName, newLastName, newPhone);

        // Assert
        Assert.Equal(newFirstName, serviceUser.FirstName);
        Assert.Equal(newLastName, serviceUser.LastName);
        Assert.Equal(newPhone, serviceUser.PhoneNumber);
    }

    [Fact]
    public void UpdateProfile_WhenDeactivated_ShouldThrowDomainException()
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.Activate();
        serviceUser.Deactivate("Test reason");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            serviceUser.UpdateProfile("Jane", "Smith"));
        Assert.Contains("deactivated", exception.Message);
    }

    [Fact]
    public void SetAvailability_WithDifferentValue_ShouldUpdateAndRaiseEvent()
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.Activate();
        serviceUser.ClearDomainEvents();

        // Act
        serviceUser.SetAvailability(false);

        // Assert
        Assert.False(serviceUser.IsAvailable);
        Assert.Single(serviceUser.DomainEvents);
        Assert.IsType<ServiceUserAvailabilityChangedEvent>(serviceUser.DomainEvents.First());
    }

    [Fact]
    public void SetAvailability_WithSameValue_ShouldNotRaiseEvent()
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.Activate();
        serviceUser.ClearDomainEvents();

        // Act - IsAvailable is true after activation
        serviceUser.SetAvailability(true);

        // Assert
        Assert.True(serviceUser.IsAvailable);
        Assert.Empty(serviceUser.DomainEvents);
    }

    [Fact]
    public void SetAvailability_WhenDeactivated_ShouldThrowDomainException()
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.Activate();
        serviceUser.Deactivate("Test reason");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            serviceUser.SetAvailability(true));
        Assert.Contains("deactivated", exception.Message);
    }

    [Fact]
    public void UpdateSpecialization_WithValidData_ShouldUpdateSpecialization()
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.Activate();

        var newSpecialization = "Mechanical Systems";

        // Act
        serviceUser.UpdateSpecialization(newSpecialization);

        // Assert
        Assert.Equal(newSpecialization, serviceUser.Specialization);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateSpecialization_WithInvalidData_ShouldThrowValidationException(string? specialization)
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.Activate();

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            serviceUser.UpdateSpecialization(specialization!));
        Assert.Contains("Specialization", exception.Errors.Keys);
    }

    [Fact]
    public void UpdateSpecialization_WhenDeactivated_ShouldThrowDomainException()
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");
        serviceUser.Activate();
        serviceUser.Deactivate("Test reason");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            serviceUser.UpdateSpecialization("New Specialization"));
        Assert.Contains("deactivated", exception.Message);
    }

    [Fact]
    public void GetFullName_ShouldReturnConcatenatedName()
    {
        // Arrange
        var serviceUser = ServiceUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com");

        // Act
        var fullName = serviceUser.GetFullName();

        // Assert
        Assert.Equal("John Doe", fullName);
    }
}
