using Microsoft.AspNetCore.Mvc;

namespace Flowertrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Simple ping endpoint to verify API is running
    /// </summary>
    [HttpGet("ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        _logger.LogInformation("Health check ping received");
        
        return Ok(new 
        { 
            Status = "Healthy",
            Message = "FLOWerTRACK API is running",
            Timestamp = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Get API version information
    /// </summary>
    [HttpGet("version")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Version()
    {
        return Ok(new
        {
            Version = "1.0.0",
            ApiName = "FLOWerTRACK API",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            DotNetVersion = Environment.Version.ToString()
        });
    }
}
