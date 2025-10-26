namespace Flowertrack.Domain.Enums;

/// <summary>
/// Represents the priority level of a ticket
/// </summary>
public enum Priority
{
    /// <summary>
    /// Low priority - can be addressed when time permits
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium priority - normal issue that needs attention
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High priority - important issue requiring quick resolution
    /// </summary>
    High = 2,

    /// <summary>
    /// Critical priority - production-stopping issue requiring immediate attention
    /// </summary>
    Critical = 3
}
