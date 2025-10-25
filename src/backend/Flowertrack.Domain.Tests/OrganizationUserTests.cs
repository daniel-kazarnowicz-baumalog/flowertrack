using Flowertrack.Api.Domain.Entities;
using Flowertrack.Api.Domain.Enums;
using Flowertrack.Api.Domain.Events;
using Flowertrack.Api.Exceptions;

namespace Flowertrack.Domain.Tests;

public class OrganizationUserTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrganizationUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var organizationId = Guid.NewGuid();
        var role = "Admin";
        var phoneNumber = "+1234567890";
        var createdBy = Guid.NewGuid();

        // Act
        var organizationUser = OrganizationUser.Create(
            userId,
            firstName,
            lastName,
            email,
            organizationId,
            role,
            phoneNumber,
            createdBy);

        // Assert
        Assert.NotNull(organizationUser);
        Assert.Equal(userId, organizationUser.Id);
        Assert.Equal(userId, organizationUser.UserId);
        Assert.Equal(firstName, organizationUser.FirstName);
        Assert.Equal(lastName, organizationUser.LastName);
        Assert.Equal(email.ToLowerInvariant(), organizationUser.Email);
        Assert.Equal(organizationId, organizationUser.OrganizationId);
        Assert.Equal(role, organizationUser.Role);
        Assert.Equal(phoneNumber, organizationUser.PhoneNumber);
        Assert.Equal(UserStatus.Pending, organizationUser.Status);
        Assert.True(organizationUser.CreatedAt > DateTime.MinValue);
        Assert.Equal(createdBy, organizationUser.CreatedBy);
    }

    [Fact]
    public void Create_WithDefaultRole_ShouldCreateWithUserRole()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();

        // Act
        var organizationUser = OrganizationUser.Create(
            userId,
            "Jane",
            "Smith",
            "jane.smith@example.com",
            organizationId);

        // Assert
        Assert.Equal("User", organizationUser.Role);
    }

    [Fact]
    public void Create_ShouldRaiseDomainEvent()
    {
        // Arrange & Act
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid());

        // Assert
        Assert.Single(organizationUser.DomainEvents);
        var domainEvent = organizationUser.DomainEvents.First();
        Assert.IsType<OrganizationUserCreatedEvent>(domainEvent);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidFirstName_ShouldThrowValidationException(string? firstName)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            OrganizationUser.Create(
                Guid.NewGuid(),
                firstName!,
                "Doe",
                "john.doe@example.com",
                Guid.NewGuid()));

        Assert.Contains("FirstName", exception.Errors.Keys);
    }

    [Fact]
    public void Create_WithEmptyOrganizationId_ShouldThrowValidationException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            OrganizationUser.Create(
                Guid.NewGuid(),
                "John",
                "Doe",
                "john.doe@example.com",
                Guid.Empty));

        Assert.Contains("OrganizationId", exception.Errors.Keys);
    }

    [Theory]
    [InlineData("InvalidRole")]
    [InlineData("SuperAdmin")]
    public void Create_WithInvalidRole_ShouldThrowValidationException(string role)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            OrganizationUser.Create(
                Guid.NewGuid(),
                "John",
                "Doe",
                "john.doe@example.com",
                Guid.NewGuid(),
                role));

        Assert.Contains("Role", exception.Errors.Keys);
    }

    [Theory]
    [InlineData("Owner")]
    [InlineData("Admin")]
    [InlineData("User")]
    [InlineData("owner")]
    [InlineData("admin")]
    [InlineData("user")]
    public void Create_WithValidRole_ShouldCreateOrganizationUser(string role)
    {
        // Arrange & Act
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid(),
            role);

        // Assert
        Assert.Equal(role, organizationUser.Role);
    }

    [Fact]
    public void Activate_WhenPending_ShouldActivateUser()
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid());
        organizationUser.ClearDomainEvents();

        // Act
        organizationUser.Activate();

        // Assert
        Assert.Equal(UserStatus.Active, organizationUser.Status);
        Assert.Null(organizationUser.DeactivationReason);
        Assert.Single(organizationUser.DomainEvents);
        Assert.IsType<OrganizationUserActivatedEvent>(organizationUser.DomainEvents.First());
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldThrowDomainException()
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid());
        organizationUser.Activate();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => organizationUser.Activate());
        Assert.Contains("already active", exception.Message);
    }

    [Fact]
    public void Deactivate_WithValidReason_ShouldDeactivateUser()
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid());
        organizationUser.Activate();
        organizationUser.ClearDomainEvents();
        var reason = "User left organization";

        // Act
        organizationUser.Deactivate(reason);

        // Assert
        Assert.Equal(UserStatus.Deactivated, organizationUser.Status);
        Assert.Equal(reason, organizationUser.DeactivationReason);
        Assert.Single(organizationUser.DomainEvents);
        Assert.IsType<OrganizationUserDeactivatedEvent>(organizationUser.DomainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Deactivate_WithInvalidReason_ShouldThrowValidationException(string? reason)
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid());
        organizationUser.Activate();

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            organizationUser.Deactivate(reason!));

        Assert.Contains("Reason", exception.Errors.Keys);
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldUpdateProfile()
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid());
        organizationUser.Activate();

        var newFirstName = "Jane";
        var newLastName = "Smith";
        var newPhone = "+9876543210";

        // Act
        organizationUser.UpdateProfile(newFirstName, newLastName, newPhone);

        // Assert
        Assert.Equal(newFirstName, organizationUser.FirstName);
        Assert.Equal(newLastName, organizationUser.LastName);
        Assert.Equal(newPhone, organizationUser.PhoneNumber);
    }

    [Fact]
    public void UpdateProfile_WhenDeactivated_ShouldThrowDomainException()
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid());
        organizationUser.Activate();
        organizationUser.Deactivate("Test reason");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            organizationUser.UpdateProfile("Jane", "Smith"));
        Assert.Contains("deactivated", exception.Message);
    }

    [Fact]
    public void ChangeOrganization_WithValidId_ShouldChangeOrganization()
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid());
        organizationUser.Activate();

        var newOrganizationId = Guid.NewGuid();

        // Act
        organizationUser.ChangeOrganization(newOrganizationId);

        // Assert
        Assert.Equal(newOrganizationId, organizationUser.OrganizationId);
    }

    [Fact]
    public void ChangeOrganization_ToSameOrganization_ShouldThrowDomainException()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            organizationId);
        organizationUser.Activate();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            organizationUser.ChangeOrganization(organizationId));
        Assert.Contains("already in this organization", exception.Message);
    }

    [Fact]
    public void ChangeOrganization_WhenDeactivated_ShouldThrowDomainException()
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid());
        organizationUser.Activate();
        organizationUser.Deactivate("Test reason");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            organizationUser.ChangeOrganization(Guid.NewGuid()));
        Assert.Contains("deactivated", exception.Message);
    }

    [Fact]
    public void UpdateRole_WithValidRole_ShouldUpdateRole()
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid(),
            "User");
        organizationUser.Activate();
        organizationUser.ClearDomainEvents();

        // Act
        organizationUser.UpdateRole("Admin");

        // Assert
        Assert.Equal("Admin", organizationUser.Role);
        Assert.Single(organizationUser.DomainEvents);
        var domainEvent = organizationUser.DomainEvents.First();
        Assert.IsType<OrganizationUserRoleChangedEvent>(domainEvent);
    }

    [Fact]
    public void UpdateRole_ToSameRole_ShouldNotRaiseEvent()
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid(),
            "User");
        organizationUser.Activate();
        organizationUser.ClearDomainEvents();

        // Act
        organizationUser.UpdateRole("User");

        // Assert
        Assert.Equal("User", organizationUser.Role);
        Assert.Empty(organizationUser.DomainEvents);
    }

    [Theory]
    [InlineData("InvalidRole")]
    [InlineData("SuperAdmin")]
    public void UpdateRole_WithInvalidRole_ShouldThrowValidationException(string role)
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid());
        organizationUser.Activate();

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() =>
            organizationUser.UpdateRole(role));
        Assert.Contains("Role", exception.Errors.Keys);
    }

    [Fact]
    public void UpdateRole_WhenDeactivated_ShouldThrowDomainException()
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid());
        organizationUser.Activate();
        organizationUser.Deactivate("Test reason");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            organizationUser.UpdateRole("Admin"));
        Assert.Contains("deactivated", exception.Message);
    }

    [Fact]
    public void GetFullName_ShouldReturnConcatenatedName()
    {
        // Arrange
        var organizationUser = OrganizationUser.Create(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            Guid.NewGuid());

        // Act
        var fullName = organizationUser.GetFullName();

        // Assert
        Assert.Equal("John Doe", fullName);
    }
}
