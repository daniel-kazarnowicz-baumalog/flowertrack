using Flowertrack.Application.Organizations.Commands.OnboardOrganization;
using Flowertrack.Application.Organizations.Queries.GetOrganizations;
using Flowertrack.Contracts.Common;
using Flowertrack.Contracts.Organizations.Requests;
using Flowertrack.Contracts.Organizations.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flowertrack.Api.Controllers;

/// <summary>
/// Controller for managing organizations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrganizationsController> _logger;

    public OrganizationsController(
        IMediator mediator,
        ILogger<OrganizationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all organizations with pagination
    /// US-024: Wyświetlenie listy organizacji z kluczowymi informacjami
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<OrganizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? serviceStatus = null)
    {
        var query = new GetOrganizationsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            ServiceStatus = serviceStatus
        };

        var result = await _mediator.Send(query);

        var response = new PaginatedResponse<OrganizationResponse>
        {
            Items = result.Items.Select(o => new OrganizationResponse
            {
                Id = o.Id,
                Name = o.Name,
                Email = o.Email,
                Phone = o.Phone,
                City = o.City,
                Country = o.Country,
                ServiceStatus = o.ServiceStatus,
                ContractStartDate = o.ContractStartDate,
                ContractEndDate = o.ContractEndDate,
                HasApiKey = o.HasApiKey,
                CreatedAt = o.CreatedAt
            }).ToList(),
            Pagination = new PaginationMetadata
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Onboard a new organization
    /// US-025: Inicjowanie onboardingu nowej organizacji
    /// </summary>
    [HttpPost("onboard")]
    [Authorize(Roles = "ServiceAdministrator")]
    [ProducesResponseType(typeof(OnboardingConfirmationResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> OnboardOrganization([FromBody] OnboardOrganizationRequest request)
    {
        var command = new OnboardOrganizationCommand
        {
            Name = request.Name,
            AdminEmail = request.AdminEmail,
            AdminFirstName = request.AdminFirstName,
            AdminLastName = request.AdminLastName,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City,
            PostalCode = request.PostalCode,
            Country = request.Country,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse(result.Error!));
        }

        return Accepted(new OnboardingConfirmationResponse
        {
            OrganizationId = result.Value,
            Message = "Invitation email sent to organization administrator",
            InvitationValidUntil = DateTimeOffset.UtcNow.AddDays(7)
        });
    }
}
