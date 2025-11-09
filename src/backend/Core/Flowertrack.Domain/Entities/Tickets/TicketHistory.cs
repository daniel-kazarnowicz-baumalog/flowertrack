using Flowertrack.Domain.Common;
using Flowertrack.Domain.Enums;

namespace Flowertrack.Domain.Entities.Tickets;

/// <summary>
/// Represents a history entry for a ticket (status changes, assignments, comments, notes, attachments)
/// </summary>
public sealed class TicketHistory : Entity<Guid>
{
    /// <summary>
    /// Ticket this history entry belongs to
    /// </summary>
    public Guid TicketId { get; private set; }

    /// <summary>
    /// Type of history entry (StatusChange, Assignment, Comment, Note, Attachment)
    /// </summary>
    public TicketHistoryType HistoryType { get; private set; }

    /// <summary>
    /// User who performed the action (ServiceUser or OrganizationUser)
    /// </summary>
    public Guid? UserId { get; private set; }

    /// <summary>
    /// Name of the user who performed the action (denormalized for display)
    /// </summary>
    public string UserName { get; private set; } = string.Empty;

    /// <summary>
    /// Type of user who performed the action ("ServiceUser" or "OrganizationUser")
    /// </summary>
    public string? UserType { get; private set; }

    /// <summary>
    /// Old value (for status changes, assignments, etc.)
    /// </summary>
    public string? OldValue { get; private set; }

    /// <summary>
    /// New value (for status changes, assignments, etc.)
    /// </summary>
    public string? NewValue { get; private set; }

    /// <summary>
    /// Comment or note content
    /// </summary>
    public string? Content { get; private set; }

    /// <summary>
    /// Attachment file name (if applicable)
    /// </summary>
    public string? AttachmentFileName { get; private set; }

    /// <summary>
    /// Attachment file path or URL (if applicable)
    /// </summary>
    public string? AttachmentFilePath { get; private set; }

    /// <summary>
    /// Attachment file size in bytes (if applicable)
    /// </summary>
    public long? AttachmentFileSize { get; private set; }

    /// <summary>
    /// Indicates if this is an internal note (visible only to service users)
    /// </summary>
    public bool IsInternal { get; private set; }

    /// <summary>
    /// When this history entry was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Navigation property to the ticket
    /// </summary>
    public Ticket? Ticket { get; private set; }

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private TicketHistory() : base(Guid.Empty)
    {
    }

    /// <summary>
    /// Creates a status change history entry
    /// </summary>
    public static TicketHistory CreateStatusChange(
        Guid ticketId,
        TicketStatus oldStatus,
        TicketStatus newStatus,
        Guid? userId = null,
        string? userName = null,
        string? userType = null)
    {
        var id = Guid.NewGuid();
        return new TicketHistory(id)
        {
            TicketId = ticketId,
            HistoryType = TicketHistoryType.StatusChange,
            UserId = userId,
            UserName = userName ?? "System",
            UserType = userType,
            OldValue = oldStatus.ToString(),
            NewValue = newStatus.ToString(),
            IsInternal = false,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Creates an assignment change history entry
    /// </summary>
    public static TicketHistory CreateAssignmentChange(
        Guid ticketId,
        string? oldAssigneeName,
        string? newAssigneeName,
        Guid? userId = null,
        string? userName = null,
        string? userType = null)
    {
        var id = Guid.NewGuid();
        return new TicketHistory(id)
        {
            TicketId = ticketId,
            HistoryType = TicketHistoryType.Assignment,
            UserId = userId,
            UserName = userName ?? "System",
            UserType = userType,
            OldValue = oldAssigneeName ?? "Unassigned",
            NewValue = newAssigneeName ?? "Unassigned",
            IsInternal = false,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Creates a comment history entry
    /// </summary>
    public static TicketHistory CreateComment(
        Guid ticketId,
        string content,
        Guid userId,
        string userName,
        string userType,
        bool isInternal = false)
    {
        var id = Guid.NewGuid();
        return new TicketHistory(id)
        {
            TicketId = ticketId,
            HistoryType = TicketHistoryType.Comment,
            UserId = userId,
            UserName = userName,
            UserType = userType,
            Content = content,
            IsInternal = isInternal,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Creates a note history entry (internal only)
    /// </summary>
    public static TicketHistory CreateNote(
        Guid ticketId,
        string content,
        Guid userId,
        string userName,
        string userType)
    {
        var id = Guid.NewGuid();
        return new TicketHistory(id)
        {
            TicketId = ticketId,
            HistoryType = TicketHistoryType.Note,
            UserId = userId,
            UserName = userName,
            UserType = userType,
            Content = content,
            IsInternal = true, // Notes are always internal
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Creates an attachment history entry
    /// </summary>
    public static TicketHistory CreateAttachment(
        Guid ticketId,
        string fileName,
        string filePath,
        long fileSize,
        Guid userId,
        string userName,
        string userType,
        bool isInternal = false)
    {
        var id = Guid.NewGuid();
        return new TicketHistory(id)
        {
            TicketId = ticketId,
            HistoryType = TicketHistoryType.Attachment,
            UserId = userId,
            UserName = userName,
            UserType = userType,
            AttachmentFileName = fileName,
            AttachmentFilePath = filePath,
            AttachmentFileSize = fileSize,
            IsInternal = isInternal,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Private constructor with ID for entity creation
    /// </summary>
    private TicketHistory(Guid id) : base(id)
    {
    }

    /// <summary>
    /// Updates the content of a comment or note
    /// </summary>
    public void UpdateContent(string newContent)
    {
        if (HistoryType != TicketHistoryType.Comment && HistoryType != TicketHistoryType.Note)
        {
            throw new InvalidOperationException("Only comments and notes can have their content updated");
        }

        Content = newContent;
    }
}
