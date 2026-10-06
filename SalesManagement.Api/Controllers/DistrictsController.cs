using Microsoft.AspNetCore.Mvc;
using SalesManagement.Application.Dtos;
using SalesManagement.Application.Exceptions;
using SalesManagement.Application.Interfaces;

namespace SalesManagement.Api.Controllers;

[ApiController]
[Route("api/districts")]
public class DistrictsController(IDistrictService districtService) : ControllerBase
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
    
    [HttpPut("{districtId:int}/salespersons/{salespersonId:int}")]
    public async Task<ActionResult> AssignSalesperson(
        int districtId,
        int salespersonId,
        [FromBody] AssignSalespersonRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await districtService.AssignSalespersonAsync(
                districtId,
                salespersonId,
                request.Role,
                cancellationToken);

            return NoContent();
        }
        catch (DistrictNotFoundException)
        {
            return NotFound();
        }
        catch (SalespersonNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{districtId:int}/salespersons/{salespersonId:int}")]
    public async Task<ActionResult> RemoveSalesperson(
        int districtId,
        int salespersonId,
        CancellationToken cancellationToken)
    {
        try
        {
            await districtService.RemoveSalespersonAsync(
                districtId,
                salespersonId,
                cancellationToken);

            return NoContent();
        }
        catch (DistrictNotFoundException)
        {
            return NotFound();
        }
        catch (SalespersonNotFoundException)
        {
            return NotFound();
        }
        catch (PrimarySalespersonRequiredException ex)
        {
            return Conflict(new
            {
                code = "PrimarySalespersonRequired",
                message = ex.Message
            });
        }
    }
}