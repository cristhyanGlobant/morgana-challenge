using Microsoft.AspNetCore.Mvc;
using UmbracoBridge.Application.Interfaces;

namespace UmbracoBridge.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthcheckController : ControllerBase
{
    private readonly IUmbracoService _umbracoService;

    public HealthcheckController(IUmbracoService umbracoService)
    {
        _umbracoService = umbracoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetHealthStatus()
    {
        var result = await _umbracoService.GetHealthCheckAsync();
        return Ok(result);
    }
}