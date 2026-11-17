using System.Data;
using Microsoft.Data.SqlClient;

namespace TeamNest.Data;

/// <summary>Creates database connections for Dapper.</summary>
public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Dapper opens the connection automatically when a query runs
    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
