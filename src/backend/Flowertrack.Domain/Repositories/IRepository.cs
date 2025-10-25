using Ardalis.Specification;
using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Base repository interface for all aggregate roots.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
public interface IRepository<T> where T : class, IAggregateRoot
{
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <typeparam name="TId">The identifier type.</typeparam>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) 
        where TId : notnull;
    
    /// <summary>
    /// Lists all entities matching the specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of entities matching the specification.</returns>
    Task<List<T>> ListAsync(ISpecification<T> specification, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists all entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all entities.</returns>
    Task<List<T>> ListAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added entity.</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts all entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total count of entities.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts entities matching the specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The count of entities matching the specification.</returns>
    Task<int> CountAsync(ISpecification<T> specification, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any entity matches the specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if any entity matches the specification; otherwise, false.</returns>
    Task<bool> AnyAsync(ISpecification<T> specification, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets first entity matching the specification or null.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The first entity matching the specification if found; otherwise, null.</returns>
    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, 
        CancellationToken cancellationToken = default);
}
