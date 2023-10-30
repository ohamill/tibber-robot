using Npgsql;
using RobotCleaner.Models.Shared;

namespace RobotCleaner.Db;

/// <summary>
/// Responsible for establishing a connection with the Postgres database and reading/writing data from/to it.
/// </summary>
public class PostgresClient : IDbClient
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresClient(string? hostName, string? username, string? password, string? databaseName)
    {
        var connectionString = $"Host={hostName};Username={username};Password={password};Database={databaseName}";
        _dataSource = NpgsqlDataSource.Create(connectionString);
    }

    public async Task<int?> Insert(ExecutionReport executionReport)
    {
        await using var cmd =
            _dataSource.CreateCommand(
                "INSERT INTO executions (Timestamp, Commands, Result, Duration) VALUES ($1, $2, $3, $4) RETURNING Id");
        cmd.Parameters.AddWithValue(executionReport.Timestamp);
        cmd.Parameters.AddWithValue(executionReport.Commands);
        cmd.Parameters.AddWithValue(executionReport.Result);
        cmd.Parameters.AddWithValue(executionReport.Duration);
        return (int?)await cmd.ExecuteScalarAsync();
    }

    public async Task<object[]?> Get(int id)
    {
        await using var cmd = _dataSource.CreateCommand("SELECT * FROM executions WHERE id = $1");
        cmd.Parameters.AddWithValue(id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null; // Return null if no rows are found

        var rowValues = new object[reader.FieldCount];
        reader.GetValues(rowValues);
        return rowValues;
    }
}