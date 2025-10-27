using Flowertrack.Application.Common.Exceptions;
using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Machines.Commands.RegisterMachine;

/// <summary>
/// Handler for RegisterMachineCommand
/// </summary>
public sealed class RegisterMachineCommandHandler
    : IRequestHandler<RegisterMachineCommand, Result<Guid>>
{
    private readonly IMachineRepository _machineRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RegisterMachineCommandHandler> _logger;

    public RegisterMachineCommandHandler(
        IMachineRepository machineRepository,
        IOrganizationRepository organizationRepository,
        ICurrentUserService currentUserService,
        ILogger<RegisterMachineCommandHandler> logger)
    {
        _machineRepository = machineRepository;
        _organizationRepository = organizationRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        RegisterMachineCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Verify organization exists
        var organization = await _organizationRepository.GetByIdAsync(
            request.OrganizationId,
            cancellationToken);

        if (organization == null)
        {
            throw new NotFoundException(nameof(Organization), request.OrganizationId);
        }

        // 2. Check if organization can register machines
        if (!organization.CanRegisterMachines())
        {
            return Result.Failure<Guid>(
                "Organization cannot register machines. Service status must be Active.");
        }

        // 3. Check if serial number already exists
        if (await _machineRepository.SerialNumberExistsAsync(
            request.SerialNumber,
            null,
            cancellationToken))
        {
            throw new ConflictException($"Machine with serial number '{request.SerialNumber}' already exists");
        }

        // 4. Create machine (Domain)
        var machine = Machine.Create(
            organizationId: request.OrganizationId,
            serialNumber: request.SerialNumber,
            brand: request.Brand,
            model: request.Model,
            location: request.Location,
            createdBy: _currentUserService.UserId
        );

        // 5. Generate API token for machine
        machine.GenerateApiToken(_currentUserService.UserId);

        // 6. Add machine to repository
        await _machineRepository.AddAsync(machine, cancellationToken);

        _logger.LogInformation(
            "Machine {MachineId} with serial number {SerialNumber} registered for organization {OrganizationId}",
            machine.Id,
            machine.SerialNumber,
            request.OrganizationId
        );

        return Result.Success(machine.Id);
    }
}
