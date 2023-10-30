namespace RobotCleaner.Models.Shared;

/// <summary>
/// Represents a single point in a 2-D space
/// </summary>
/// <param name="X">The location's x-axis</param>
/// <param name="Y">The location's y-axis</param>
public readonly record struct Coordinate(int X, int Y);