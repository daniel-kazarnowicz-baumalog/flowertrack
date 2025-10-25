using Flowertrack.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrganizationsController> _logger;

    public OrganizationsController(
        ApplicationDbContext context,
        ILogger<OrganizationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all organizations
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Fetching all organizations");
        
        var organizations = await _context.Organizations
            .OrderBy(o => o.Name)
            .Select(o => new
            {
                o.Id,
                o.Name,
                o.Email,
                o.Phone,
                o.City,
                o.Country,
                o.ServiceStatus,
                o.ContractStartDate,
                o.CreatedAt,
                HasApiKey = !string.IsNullOrEmpty(o.ApiKey)
            })
            .ToListAsync();

        return Ok(organizations);
    }

    /// <summary>
    /// Get organization by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation("Fetching organization with ID: {OrganizationId}", id);
        
        var organization = await _context.Organizations
            .Where(o => o.Id == id)
            .Select(o => new
            {
                o.Id,
                o.Name,
                o.Email,
                o.Phone,
                o.Address,
                o.City,
                o.PostalCode,
                o.Country,
                o.ServiceStatus,
                o.ContractStartDate,
                o.ContractEndDate,
                o.Notes,
                o.CreatedAt,
                o.UpdatedAt,
                HasApiKey = !string.IsNullOrEmpty(o.ApiKey)
            })
            .FirstOrDefaultAsync();

        if (organization == null)
        {
            return NotFound(new { Message = $"Organization with ID {id} not found" });
        }

        return Ok(organization);
    }

    /// <summary>
    /// Get organizations count
    /// </summary>
    [HttpGet("count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCount()
    {
        var count = await _context.Organizations.CountAsync();
        
        return Ok(new
        {
            Total = count,
            Active = await _context.Organizations.CountAsync(o => o.ServiceStatus == ServiceStatus.Active),
            Suspended = await _context.Organizations.CountAsync(o => o.ServiceStatus == ServiceStatus.Suspended),
            Expired = await _context.Organizations.CountAsync(o => o.ServiceStatus == ServiceStatus.Expired)
        });
    }
}
