using System.Diagnostics;

namespace RobotCleaner.Shared;

public static class Timer
{
    /// <summary>
    /// Measures the execution time of the supplied action
    /// </summary>
    /// <param name="a">The action whose execution time will be measured</param>
    /// <returns>The length of time (in seconds) the action takes to execute</returns>
    public static double MeasureDuration(Action a)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        a();

        stopwatch.Stop();
        return stopwatch.Elapsed.TotalSeconds;
    }
}