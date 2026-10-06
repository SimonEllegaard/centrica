using Microsoft.AspNetCore.Mvc;
using SalesManagement.Application.Interfaces;

namespace SalesManagement.Api.Controllers;

[ApiController]
[Route("api/salespersons")]
public class SalespersonsController(
    ISalespersonService salespersonService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetSalespeople(
        CancellationToken cancellationToken)
    {
        var salespeople = await salespersonService.GetSalespeopleAsync(
            cancellationToken);

        return Ok(salespeople);
    }
}