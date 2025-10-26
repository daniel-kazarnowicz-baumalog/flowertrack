using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for Ticket aggregate root.
/// Provides specialized query methods for ticket management.
/// </summary>
public interface ITicketRepository : IRepository<Ticket>
{
    /// <summary>
    /// Gets a ticket by its unique ticket number.
    /// </summary>
    /// <param name="ticketNumber">The ticket number to search for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The ticket if found; otherwise, null.</returns>
    Task<Ticket?> GetByTicketNumberAsync(TicketNumber ticketNumber, CancellationToken ct = default);

    /// <summary>
    /// Gets all tickets for a specific organization.
    /// </summary>
    /// <param name="organizationId">The organization identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of tickets.</returns>
    Task<IReadOnlyList<Ticket>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken ct = default);

    /// <summary>
    /// Gets all tickets for a specific machine.
    /// </summary>
    /// <param name="machineId">The machine identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of tickets.</returns>
    Task<IReadOnlyList<Ticket>> GetByMachineIdAsync(Guid machineId, CancellationToken ct = default);

    /// <summary>
    /// Gets all tickets assigned to a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of tickets.</returns>
    Task<IReadOnlyList<Ticket>> GetByAssignedUserIdAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Gets all tickets with a specific status.
    /// </summary>
    /// <param name="status">The ticket status to filter by.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of tickets.</returns>
    Task<IReadOnlyList<Ticket>> GetByStatusAsync(TicketStatus status, CancellationToken ct = default);

    /// <summary>
    /// Gets the next sequential number for a ticket in a specific year.
    /// </summary>
    /// <param name="year">The year for which to get the next number.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The next sequential number (starting from 1).</returns>
    Task<int> GetNextSequentialNumberAsync(int year, CancellationToken ct = default);

    /// <summary>
    /// Checks if a ticket number already exists.
    /// </summary>
    /// <param name="ticketNumber">The ticket number to check.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the ticket number exists; otherwise, false.</returns>
    Task<bool> TicketNumberExistsAsync(TicketNumber ticketNumber, CancellationToken ct = default);
}
