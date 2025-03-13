using Microsoft.AspNetCore.Mvc;
using SommusProject.Repositories;
using SommusProject.Services;

namespace SommusProject.Controllers;

[ApiController]
[Route("[controller]")]
public class AlertDengueController : ControllerBase
{
    private readonly AlertDengueService _dengueService;
    private readonly AlertDengueRepository _repository;
    public AlertDengueController(AlertDengueService dengueService,
        AlertDengueRepository repository)
    {
        _dengueService = dengueService;
        _repository = repository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetService()
    {
        try
        {
            var alerts = await _dengueService.GetAlertsDengue();
            
            if (alerts is null)
                return NotFound();
            
            return Ok(alerts);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("")]
    public async Task<IActionResult> PostService()
    {
        try
        {
            var alerts = await _dengueService.GetAlertsDengue();
            
            if (alerts is null)
                return NotFound();
            
            var resultado = await _repository.SalvarAlertasDengue(alerts);
            
            if (!resultado)
                return BadRequest("Erro ao salvar alertas");
            
            return Created($"{Request.Path}/alertas", alerts);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}