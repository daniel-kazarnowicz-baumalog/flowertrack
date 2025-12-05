using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Comments;

public record UpdateCommentRequest(
    [Required] string Content
);
