using FluentValidation;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;

namespace Flowertrack.Application.Tickets.Commands.CreateTicket;

/// <summary>
/// Validator for CreateTicketCommand
/// Validates organization, machine, title, description, priority, and user permissions
/// </summary>
public sealed class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IOrganizationUserRepository _userRepository;

    public CreateTicketCommandValidator(
        IOrganizationRepository organizationRepository,
        IMachineRepository machineRepository,
        IOrganizationUserRepository userRepository)
    {
        _organizationRepository = organizationRepository;
        _machineRepository = machineRepository;
        _userRepository = userRepository;

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Organization ID is required")
            .MustAsync(OrganizationExists)
            .WithMessage("Organization does not exist");

        RuleFor(x => x.MachineId)
            .NotEmpty().WithMessage("Machine ID is required")
            .MustAsync(MachineExists)
            .WithMessage("Machine does not exist");

        RuleFor(x => x)
            .MustAsync(MachineBelongsToOrganization)
            .WithMessage("Machine does not belong to the specified organization");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("Created by user ID is required")
            .MustAsync(UserExists)
            .WithMessage("User does not exist");

        RuleFor(x => x)
            .MustAsync(UserHasPermissionToCreateTicket)
            .WithMessage("User does not have permission to create ticket for this organization");
    }

    private async Task<bool> OrganizationExists(Guid organizationId, CancellationToken cancellationToken)
    {
        return await _organizationRepository.ExistsAsync(organizationId, cancellationToken);
    }

    private async Task<bool> MachineExists(Guid machineId, CancellationToken cancellationToken)
    {
        return await _machineRepository.ExistsAsync(machineId, cancellationToken);
    }

    private async Task<bool> MachineBelongsToOrganization(
        CreateTicketCommand command,
        CancellationToken cancellationToken)
    {
        var machine = await _machineRepository.GetByIdAsync(command.MachineId, cancellationToken);
        return machine?.OrganizationId == command.OrganizationId;
    }

    private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user != null;
    }

    private async Task<bool> UserHasPermissionToCreateTicket(
        CreateTicketCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.CreatedBy, cancellationToken);
        
        if (user == null)
        {
            return false;
        }

        // User can create ticket if they belong to the same organization
        // TODO: Add service team member check when service team functionality is implemented
        return user.OrganizationId == command.OrganizationId;
    }
}
