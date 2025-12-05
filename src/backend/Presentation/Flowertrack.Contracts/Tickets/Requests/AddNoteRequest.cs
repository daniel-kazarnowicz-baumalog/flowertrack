using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Tickets.Requests;

public record AddNoteRequest(
    [Required] string Content
);
