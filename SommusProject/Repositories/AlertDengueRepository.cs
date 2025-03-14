using Dapper;
using Dapper.Contrib.Extensions;
using SommusProject.Data;
using SommusProject.Models;

namespace SommusProject.Repositories;

public class AlertDengueRepository : IAlertDengueRepository
{
    private readonly AlertDengueDbContext _context;
    private readonly ILogger<AlertDengueRepository> _logger;
    
    public AlertDengueRepository(
        AlertDengueDbContext context,
        ILogger<AlertDengueRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<IEnumerable<AlertDengue>> GetAllAlertsDengue()
    {
        try
        {
            return await _context.Connection().GetAllAsync<AlertDengue>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter alertas de dengue: {Mensagem}", e.Message);
            throw;
        }
    }
    
    public async Task<IEnumerable<long>> GetAllAlertsDengueId()
    {
        try
        {
            return await _context.Connection().QueryAsync<long>("SELECT Identificador FROM ALERTA_DENGUE");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao obter IDs de alertas de dengue: {Mensagem}", e.Message);
            throw;
        }
    }

    public async Task<bool> SalvarAlertasDengue(List<AlertDengue> alertsDengues)
    {
        if (alertsDengues.Count > 0 == false)
            throw new ArgumentNullException(nameof(alertsDengues));

        try
        {
            var linhasAferadas = await _context.Connection().InsertAsync(alertsDengues);

            return linhasAferadas > 0;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao salvar alertas de dengue: {Mensagem}", e.Message);
            throw;
        }
    }
    
    public async Task<AlertDengue?> ConsultarPorSemanaAno(int semanaEpidemiologica, int ano)
    {
        if (semanaEpidemiologica <= 0 || semanaEpidemiologica > 53)
            throw new ArgumentOutOfRangeException(nameof(semanaEpidemiologica));

        if (ano < 1000 || ano > 9999)
            throw new ArgumentOutOfRangeException(nameof(ano));
        
        try
        {
            var parameters = new
            {
                AnoSemana = Convert.ToInt32($"{ano}{semanaEpidemiologica:D2}")
            };

            return await _context.ConsultarPrimeiroAsync<AlertDengue>("GetAlertasDengueByWeek", parameters);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao consultar alerta para semana {Semana} do ano {Ano}: {Mensagem}", 
                semanaEpidemiologica, ano, e.Message);
            throw;
        }
    }
}