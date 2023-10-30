using RobotCleaner.Models.Shared;

namespace RobotCleaner.Db;

public interface IDbHandler
{
    /// <summary>
    /// Inserts an execution report into the database
    /// </summary>
    /// <param name="executionReport">The execution report to insert into the database</param>
    /// <returns>The unique ID of the newly-inserted execution report</returns>
    Task<int?> InsertExecutionReport(ExecutionReport executionReport);

    /// <summary>
    /// Gets an execution report from the database matching the supplied ID
    /// </summary>
    /// <param name="id">The unique ID of the execution report</param>
    /// <returns>The execution report matching the supplied ID, if it exists in the database, else null</returns>
    Task<ExecutionReport?> GetExecutionReport(int id);
}