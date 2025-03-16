using System.Data;

namespace SommusProject.Data;

public interface IAlertDengueDbContext : IDisposable
{
    IDbConnection Connection();
    Task<T?> ConsultarPrimeiroAsync<T>(string storedProcedure, object parameters);
}