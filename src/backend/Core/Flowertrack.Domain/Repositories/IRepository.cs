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
    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) 
        where TId : notnull;
    
    /// <summary>
    /// Lists all entities matching the specification.
    /// </summary>
    Task<List<T>> ListAsync(ISpecification<T> specification, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists all entities.
    /// </summary>
    Task<List<T>> ListAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an entity.
    /// </summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts all entities.
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts entities matching the specification.
    /// </summary>
    Task<int> CountAsync(ISpecification<T> specification, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any entity matches the specification.
    /// </summary>
    Task<bool> AnyAsync(ISpecification<T> specification, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets first entity matching the specification or null.
    /// </summary>
    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, 
        CancellationToken cancellationToken = default);
}
