using RobotCleaner.Models.Shared;

namespace RobotCleaner.Models.Response;

public record PathResponse(int Id, DateTime Timestamp, int Commands, int Result, double Duration)
{
    public static PathResponse FromExecutionReport(ExecutionReport executionReport)
    {
        return new PathResponse(
            executionReport.Id!.Value,
            executionReport.Timestamp,
            executionReport.Commands,
            executionReport.Result,
            executionReport.Duration
            );
    }
}