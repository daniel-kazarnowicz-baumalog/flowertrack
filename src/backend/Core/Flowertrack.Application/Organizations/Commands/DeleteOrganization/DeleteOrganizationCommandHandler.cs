using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;

namespace Flowertrack.Application.Organizations.Commands.DeleteOrganization;

/// <summary>
/// Handler for DeleteOrganizationCommand
/// </summary>
public sealed class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand, Result>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IMachineRepository machineRepository,
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        _machineRepository = machineRepository ?? throw new ArgumentNullException(nameof(machineRepository));
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<Result> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        // Get organization
        var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization == null)
        {
            return Result.Failure($"Organization with ID {request.OrganizationId} was not found");
        }

        // Check if organization has active tickets
        var hasActiveTickets = await _ticketRepository.HasActiveTicketsForOrganizationAsync(request.OrganizationId, cancellationToken);
        if (hasActiveTickets)
        {
            return Result.Failure("Cannot delete organization with active tickets. Please close or resolve all tickets first.");
        }

        // Get the current user ID for audit
        var userId = _currentUserService.UserId ?? Guid.Empty;

        // Soft delete the organization
        organization.Delete(userId);

        await _organizationRepository.UpdateAsync(organization, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
