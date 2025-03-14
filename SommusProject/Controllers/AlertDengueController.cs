using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using SommusProject.Models;
using SommusProject.Repositories;
using SommusProject.Services;

namespace SommusProject.Controllers;

[ApiController]
[Route("[controller]")]
public class AlertDengueController : ControllerBase
{
    private readonly IAlertDengueService _dengueService;
    private readonly IAlertDengueRepository _repository;
    private readonly ILogger<AlertDengueController> _logger;
    public AlertDengueController(IAlertDengueService dengueService,
        IAlertDengueRepository repository,
        ILogger<AlertDengueController> logger)
    {
        _dengueService = dengueService;
        _repository = repository;
        _logger = logger;
    }
    
    private ObjectResult ProcessarErro(Exception e, string mensagem)
    {
        _logger.LogError(e, "{Mensagem}: {ExceptionMessage}", mensagem, e.Message);
        return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao processar sua solicitação. Por favor, tente novamente mais tarde.");
    }

    [HttpGet("Service")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AlertDengue>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AlertDengue>>> GetAlertsDengue()
    {
        try
        {
            var alerts = await _dengueService.GetAlertsDengue();
            
            return alerts?.Any() ?? false ? Ok(alerts) : NotFound("Nenhum dado encontrado.");
        }
        catch (Exception e)
        {
            return ProcessarErro(e, "Erro ao obter alertas de dengue.");
        }
    }
    
    [HttpPost("Service")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IEnumerable<AlertDengue>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ImportAlertsDengue()
    {
        try
        {
            var alerts = (await _dengueService.GetNewAlertsDengue())?.ToList();
            
            if (alerts?.Count > 0 == false)
                return NotFound("Nenhum novo alerta dengue para ser cadastrado.");
            
            var resultado = await _repository.SalvarAlertasDengue(alerts);
            
            if (!resultado)
                return BadRequest("Não foi possível salvar os alertas.");
            
            return Created($"{Request.Path}", alerts);
        }
        catch (Exception e)
        {
            return ProcessarErro(e, "Erro ao consultar e salvar alertas de dengue.");
        }
    }

    [HttpGet("GetByWeek")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AlertDengue))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AlertDengue>> GetByWeek([FromQuery] int ew, [FromQuery] int ey)
    {
        try
        {
            ValidarSemanaAno(ew, ey);
            
            var alert = await _repository.ConsultarPorSemanaAno(ew, ey);

            return alert is not null ? Ok(alert) : NotFound($"Nenhum alerta encontrado para o período especificado: semana {ew} do ano {ey}.");
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return ProcessarErro(e, $"Erro ao consultar alerta para semana {ew} do ano {ey}");
        }
    }

    private void ValidarSemanaAno(int ew, int ey)
    {
        if (ey < 1000 || ey > 9999)
        {
            throw new ArgumentException("Ano inválido. O ano deve ter exatamente 4 dígitos.");
        }
        
        var semanasNoAno = ISOWeek.GetWeeksInYear(ey);
        if (ew < 1 || ew > semanasNoAno)
        {
            throw new ArgumentException($"Semana epidemiológica inválida para o ano {ey}. A semana deve estar entre 1 e {semanasNoAno}.");
        }
    }
}