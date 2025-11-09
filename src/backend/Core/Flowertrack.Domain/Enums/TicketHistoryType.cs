namespace Flowertrack.Domain.Enums;

/// <summary>
/// Type of ticket history entry
/// </summary>
public enum TicketHistoryType
{
    /// <summary>
    /// Status change (e.g., Open -> InProgress)
    /// </summary>
    StatusChange = 1,

    /// <summary>
    /// Assignment change (e.g., assigning to a technician)
    /// </summary>
    Assignment = 2,

    /// <summary>
    /// Comment added (visible to both service and organization users)
    /// </summary>
    Comment = 3,

    /// <summary>
    /// Internal note added (visible only to service users)
    /// </summary>
    Note = 4,

    /// <summary>
    /// Attachment added
    /// </summary>
    Attachment = 5
}
