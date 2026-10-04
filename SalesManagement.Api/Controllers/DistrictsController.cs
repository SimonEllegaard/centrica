using Microsoft.AspNetCore.Mvc;
using SalesManagement.Application.Interfaces;

namespace SalesManagement.Api.Controllers;

[ApiController]
[Route("api/districts")]
public sealed class DistrictsController(IDistrictService districtService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetDistricts(
        CancellationToken cancellationToken)
    {
        var districts = await districtService.GetDistrictsAsync(
            cancellationToken);

        return Ok(districts);
    }

    [HttpGet("{districtId:int}")]
    public async Task<ActionResult> GetDistrict(
        int districtId,
        CancellationToken cancellationToken)
    {
        var district = await districtService.GetDistrictDetailsAsync(
            districtId,
            cancellationToken);

        if (district is null)
        {
            return NotFound();
        }

        return Ok(district);
    }
}