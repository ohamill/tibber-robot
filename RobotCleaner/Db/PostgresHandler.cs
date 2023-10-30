using Newtonsoft.Json;
using RobotCleaner.Models.Shared;

namespace RobotCleaner.Db;

/// <summary>
/// Provides a high-level interface to the Postgres database
/// </summary>
public class PostgresHandler : IDbHandler
{
    /// <summary>
    /// The DB client used by the service to interact with the database
    /// </summary>
    private readonly IDbClient _client;
    private readonly ILogger<PostgresHandler> _logger;

    public PostgresHandler(IDbClient client, ILogger<PostgresHandler> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    public async Task<int?> InsertExecutionReport(ExecutionReport executionReport)
    {
        _logger.LogInformation("Inserting execution report into database: {Report}", JsonConvert.SerializeObject(executionReport));
        return await _client.Insert(executionReport);
    }

    public async Task<ExecutionReport?> GetExecutionReport(int id)
    {
        _logger.LogInformation("Getting execution report with ID: {Id}", id);
        var rowValues = await _client.Get(id);
        return rowValues is null ? null : ParseRow(rowValues);
    }

    /// <summary>
    /// Parses a database row into an <c>ExecutionReport</c> object
    /// </summary>
    /// <param name="values">An array of values from the database row</param>
    /// <returns>An <c>ExecutionReport</c> object containing the data from the database row</returns>
    private static ExecutionReport ParseRow(object[] values)
    {
        var rowId = (int)values[0];
        var timestamp = (DateTime)values[1];
        var commands = (int)values[2];
        var result = (int)values[3];
        var duration = (double)values[4];

        return new ExecutionReport(timestamp, commands, result, duration)
        {
            Id = rowId
        };
    }
}