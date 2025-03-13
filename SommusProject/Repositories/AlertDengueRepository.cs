using Dapper;
using Dapper.Contrib.Extensions;
using SommusProject.Data;
using SommusProject.Models;

namespace SommusProject.Repositories;

public class AlertDengueRepository
{
    private readonly AlertDengueDbContext _context;

    public AlertDengueRepository(AlertDengueDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SalvarAlertasDengue(IEnumerable<AlertDengue> alertsDengues)
    {
        try
        {
            var linhasAferadas = await _context.Connection().InsertAsync(alertsDengues);

            return linhasAferadas > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao salvar alertas: {e.Message}");
            Console.WriteLine(e.StackTrace);
            throw new Exception($"Erro ao salvar alertas: {e.Message}");
        }
    }
}