using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace DotnetAPI.Data;

public class DataContextDapper
{
    private readonly IConfiguration _configuration;
    private readonly string? _connectionString;

    public DataContextDapper(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public string GetConnectionString()
    {
        return _connectionString!;
    }

    public IEnumerable<T> LoadData<T>(string sql, object? param = null)
    {
        IDbConnection connection = new SqlConnection(_connectionString);
        return connection.Query<T>(sql, param);
    }

    public T LoadSingleData<T>(string sql, object? param = null)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QuerySingleOrDefault<T>(sql, param)!;
        }
    }

    public bool ExecuteSql(string sql, object? param = null, CommandType? command = null)
    {
        IDbConnection connection = new SqlConnection(_connectionString);
        return connection.Execute(sql, param, commandType: command) > 0;
    }

    public int ExecuteSqlWithRowCount(string sql, object? param = null)
    {
        IDbConnection connection = new SqlConnection(_connectionString);
        return connection.Execute(sql, param);
    }

}
