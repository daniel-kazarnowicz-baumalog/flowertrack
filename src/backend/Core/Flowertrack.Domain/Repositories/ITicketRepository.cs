using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for Ticket aggregate.
/// </summary>
public interface ITicketRepository : IRepository<Ticket>
{
    /// <summary>
    /// Gets a ticket by its unique ticket number.
    /// </summary>
    Task<Ticket?> GetByNumberAsync(TicketNumber number, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all tickets for a specific organization.
    /// </summary>
    Task<List<Ticket>> GetByOrganizationAsync(Guid orgId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all tickets assigned to a specific service user.
    /// </summary>
    Task<List<Ticket>> GetAssignedToUserAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts tickets by status.
    /// </summary>
    Task<int> CountByStatusAsync(TicketStatus status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a ticket with all related entities loaded (organization, machine, users).
    /// </summary>
    Task<Ticket?> GetWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
