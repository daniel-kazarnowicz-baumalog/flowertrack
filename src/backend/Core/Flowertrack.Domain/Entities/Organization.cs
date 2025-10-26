using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Entities;

/// <summary>
/// Placeholder entity for Organization aggregate root.
/// Full implementation will be provided in Phase 1.1 (Issue #3).
/// </summary>
public class Organization : IAggregateRoot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? ApiKey { get; set; }
    public DateTimeOffset? ContractStartDate { get; set; }
    public DateTimeOffset? ContractEndDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
