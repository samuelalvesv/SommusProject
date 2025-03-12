using Microsoft.AspNetCore.Mvc;
using SommusProject.Services;

namespace SommusProject.Controllers;

[ApiController]
[Route("[controller]")]
public class AlertDengueController : ControllerBase
{
    private readonly AlertDengueService _dengueService;

    public AlertDengueController(AlertDengueService dengueService)
    {
        _dengueService = dengueService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Get()
    {
        var alerts = await _dengueService.GetDengueAlerts();
        return Ok(alerts);
    }

}