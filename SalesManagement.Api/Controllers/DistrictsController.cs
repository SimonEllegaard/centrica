using Microsoft.AspNetCore.Mvc;
using SalesManagement.Application.Interfaces;

namespace SalesManagement.Api.Controllers;

[ApiController]
[Route("api/districts")]
public sealed class DistrictsController : ControllerBase
{
    private readonly IDistrictService _districtService;

    public DistrictsController(IDistrictService districtService)
    {
        _districtService = districtService;
    }

    [HttpGet]
    public async Task<ActionResult> GetDistricts(
        CancellationToken cancellationToken)
    {
        var districts = await _districtService.GetDistrictsAsync(
            cancellationToken);

        return Ok(districts);
    }

    [HttpGet("{districtId:int}")]
    public async Task<ActionResult> GetDistrict(
        int districtId,
        CancellationToken cancellationToken)
    {
        var district = await _districtService.GetDistrictDetailsAsync(
            districtId,
            cancellationToken);

        if (district is null)
        {
            return NotFound();
        }

        return Ok(district);
    }
}