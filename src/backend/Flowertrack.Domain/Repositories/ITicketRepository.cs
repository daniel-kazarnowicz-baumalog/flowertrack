using Flowertrack.Domain.Entities.Tickets;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for Ticket aggregate.
/// </summary>
public interface ITicketRepository : IRepository<Ticket>
{
    /// <summary>
    /// Gets a ticket by its unique ticket number.
    /// </summary>
    /// <param name="number">The ticket number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ticket if found; otherwise, null.</returns>
    Task<Ticket?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all tickets for a specific organization.
    /// </summary>
    /// <param name="orgId">The organization identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of tickets for the organization.</returns>
    Task<List<Ticket>> GetByOrganizationAsync(Guid orgId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all tickets assigned to a specific service user.
    /// </summary>
    /// <param name="userId">The service user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of tickets assigned to the user.</returns>
    Task<List<Ticket>> GetAssignedToUserAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts tickets by status.
    /// </summary>
    /// <param name="status">The ticket status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The count of tickets with the specified status.</returns>
    Task<int> CountByStatusAsync(string status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a ticket with all related entities loaded (organization, machine, users).
    /// </summary>
    /// <param name="id">The ticket identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ticket with full details if found; otherwise, null.</returns>
    Task<Ticket?> GetWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
