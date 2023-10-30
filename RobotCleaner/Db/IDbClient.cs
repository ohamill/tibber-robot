using RobotCleaner.Models.Shared;

namespace RobotCleaner.Db;

public interface IDbClient
{
    /// <summary>
    /// Inserts an execution report into the database
    /// </summary>
    /// <param name="executionReport">The execution report to be written to the database</param>
    /// <returns>The ID of the newly-entered row in the database</returns>
    Task<int?> Insert(ExecutionReport executionReport);

    /// <summary>
    /// Gets a database row matching the supplied ID, if one exists
    /// </summary>
    /// <param name="id">The unique ID of the execution report</param>
    /// <returns>An array of all values in the database row identified by the supplied ID, if it exists, else null</returns>
    Task<object[]?> Get(int id);
}