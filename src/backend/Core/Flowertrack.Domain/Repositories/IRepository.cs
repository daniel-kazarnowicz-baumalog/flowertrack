namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Base repository interface for all entities.
/// Provides common CRUD operations for entity management.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of all entities.</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The added entity.</returns>
    Task<T> AddAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="ct">Cancellation token.</param>
    Task UpdateAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Checks if an entity with the specified identifier exists.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the entity exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
