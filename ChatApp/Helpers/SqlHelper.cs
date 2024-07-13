using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace ChatApp.Helpers;

public static class SqlHelper
{
    private static readonly string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=\Database\Database1.mdf;Integrated Security=True";

    private static readonly SqlConnectionStringBuilder SqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString)
    {
        MaxPoolSize = 100,
        MinPoolSize = 1,
        Pooling = true
    };

    /// <summary>
    /// Execute a query that return no results
    /// </summary>
    /// <param name="query"></param>
    /// <param name="parameters"></param>
    public static async Task<int> ExecuteNonQueryAsync(string query, params SqlParameter[] parameters)
    {
        try
        {
            using SqlConnection connection = new(SqlConnectionStringBuilder.ConnectionString);
            await connection.OpenAsync();
            using SqlCommand command = new(query, connection);

            command.Parameters.AddRange(parameters);
            return await command.ExecuteNonQueryAsync();
        }
        catch (SqlException ex)
        {
            Debug.WriteLine($"SQL Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Execute a query that returns a single value
    /// </summary>
    /// <param name="query"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static async Task<object> ExecuteScalarAsync(string query, params SqlParameter[] parameters)
    {
        try
        {
            using SqlConnection connection = new(SqlConnectionStringBuilder.ConnectionString);
            await connection.OpenAsync();
            using SqlCommand command = new(query, connection);

            command.Parameters.AddRange(parameters);
            return await command.ExecuteScalarAsync();
        }
        catch (SqlException ex)
        {
            Debug.WriteLine($"SQL Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Execute a query that returns a data reader
    /// </summary>
    /// <param name="query"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static async Task<SqlDataReader> ExecuteReaderAsync(string query, params SqlParameter[] parameters)
    {
        try
        {
            using SqlConnection connection = new(SqlConnectionStringBuilder.ConnectionString);
            await connection.OpenAsync();
            using SqlCommand command = new(query, connection);

            command.Parameters.AddRange(parameters);
            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection); // Ensures that the connection is closed when the reader is closed
        }
        catch (SqlException ex)
        {
            Debug.WriteLine($"SQL Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Helper method to create parameters
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static SqlParameter CreateParameter(string name, object value, SqlDbType type)
    {
        SqlParameter parameter = new(name, type);
        parameter.Value = value ?? DBNull.Value;
        return parameter;
    }
}