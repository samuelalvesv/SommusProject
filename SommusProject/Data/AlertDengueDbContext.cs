using System.Data;
using MySql.Data.MySqlClient;

namespace SommusProject.Data;

public class AlertDengueDbContext : IDisposable
{
    private readonly IDbConnection _connection;

    public AlertDengueDbContext(IConfiguration configuration)
    {
        _connection = new MySqlConnection(
            configuration.GetConnectionString("MySqlConnection"));
        _connection.Open();
    }
    
    public void Dispose()
    {
        _connection?.Dispose();
    }
}