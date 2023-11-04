using Newtonsoft.Json;

namespace RobotCleaner.Models.Shared;

/// <summary>
/// Represents the result of a robot executing a series of commands
/// </summary>
/// <param name="Timestamp">The time of the execution</param>
/// <param name="Commands">The number of commands executed</param>
/// <param name="Result">The number of unique locations visited</param>
/// <param name="Duration">The length of time taken to execute the commands</param>
public record ExecutionReport(DateTime Timestamp, int Commands, long Result, double Duration)
{
    /// <summary>
    /// The unique ID, which is only populated after the execution report has been written to the database.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? Id { get; init; }
}