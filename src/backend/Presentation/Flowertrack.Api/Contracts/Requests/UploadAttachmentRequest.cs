using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Api.Contracts.Requests;

public class UploadAttachmentRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;
}
