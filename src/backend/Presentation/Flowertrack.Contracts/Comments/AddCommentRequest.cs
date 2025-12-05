using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Comments;

public record AddCommentRequest(
    [Required] string Content,
    bool IsInternal = false
);
