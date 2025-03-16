using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace SommusProject.Data;

public class AlertDengueDbContext : IAlertDengueDbContext
{
    private readonly IDbConnection _connection;

    public AlertDengueDbContext(IConfiguration configuration)
    {
        _connection = new MySqlConnection(
            configuration.GetConnectionString("MySqlConnection"));
    }
    
    public IDbConnection Connection() => _connection;
    
    public async Task<T?> ConsultarPrimeiroAsync<T>(string storedProcedure, object parameters)
    {
        return await _connection.QueryFirstOrDefaultAsync<T>(
            storedProcedure, 
            parameters, 
            commandType: CommandType.StoredProcedure);
    }
    
    public void Dispose()
    {
        _connection.Dispose();
    }
}