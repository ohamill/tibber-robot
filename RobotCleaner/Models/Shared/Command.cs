namespace RobotCleaner.Models.Shared;

/// <summary>
/// The smallest unit of instruction to a robot, indicating how many steps to move in which direction.
/// </summary>
/// <param name="Direction">The direction the robot should move in. Valid values are: North, South, East, West</param>
/// <param name="Steps">The number of steps to move</param>
public record Command(Direction Direction, int Steps);
