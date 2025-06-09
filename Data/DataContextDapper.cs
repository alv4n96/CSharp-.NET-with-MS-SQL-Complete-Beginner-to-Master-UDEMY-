using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace HelloWorld.Data;

public class DataContextDapper
{
    private string? _connectionString = "Server=localhost;Database=DotNetCourseDB;TrustServerCertificate=true;Trusted_Connection=true";

    public IEnumerable<T> LoadData<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_connectionString);
        return dbConnection.Query<T>(sql);
    }

    public T LoadDataSingle<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_connectionString);
        return dbConnection.QuerySingle<T>(sql);
    }

    public bool ExecuteSql(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_connectionString);
        return dbConnection.Execute(sql) > 0;
    }

    public bool ExecuteSql<T>(string sql, T parameterObject)
    {
        IDbConnection dbConnection = new SqlConnection(_connectionString);
        return dbConnection.Execute(sql, parameterObject) > 0;
    }

    public int ExecuteSqlWithRowCount(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_connectionString);
        return dbConnection.Execute(sql);
    }

    public int ExecuteSqlWithRowCount<T>(string sql, T parameterObject)
    {
        IDbConnection dbConnection = new SqlConnection(_connectionString);
        return dbConnection.Execute(sql, parameterObject);
    }

}
