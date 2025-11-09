using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using Flowertrack.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Tickets.Commands.CreateTicket;

/// <summary>
/// Handler for CreateTicketCommand
/// Creates a new service ticket with automatic ticket number generation
/// </summary>
public sealed class CreateTicketCommandHandler
    : IRequestHandler<CreateTicketCommand, Result<Guid>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateTicketCommandHandler> _logger;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateTicketCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateTicketCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Creating ticket for organization {OrganizationId}, machine {MachineId}, created by {UserId}",
                request.OrganizationId,
                request.MachineId,
                request.CreatedBy);

            // Generate unique ticket number
            var currentYear = DateTime.UtcNow.Year;
            var nextSequential = await _ticketRepository.GetNextSequentialNumberAsync(
                currentYear, 
                cancellationToken);

            var ticketNumber = TicketNumber.Create(currentYear, nextSequential);

            // Verify ticket number doesn't exist (safety check)
            var exists = await _ticketRepository.TicketNumberExistsAsync(ticketNumber, cancellationToken);
            if (exists)
            {
                _logger.LogError("Generated ticket number {TicketNumber} already exists", ticketNumber.Value);
                return Result.Failure<Guid>("Failed to generate unique ticket number. Please try again.");
            }

            // Create ticket using domain factory method
            var ticket = Ticket.Create(
                ticketNumber: ticketNumber,
                title: request.Title,
                description: request.Description,
                organizationId: request.OrganizationId,
                machineId: request.MachineId,
                priority: request.Priority,
                createdByUserId: request.CreatedBy);

            // Add ticket to repository
            await _ticketRepository.AddAsync(ticket, cancellationToken);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully created ticket {TicketId} with number {TicketNumber}",
                ticket.Id,
                ticketNumber.Value);

            return Result.Success(ticket.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ticket for organization {OrganizationId}", request.OrganizationId);
            return Result.Failure<Guid>($"Failed to create ticket: {ex.Message}");
        }
    }
}
