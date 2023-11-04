namespace RobotCleaner.Models.Shared;

/// <summary>
/// Represents a 2-D line
/// </summary>
/// <param name="Start">The line's starting coordinates</param>
/// <param name="End">The line's ending coordinates</param>
/// <param name="Direction">The line's direction (valid values are North, South, East, West)</param>
public record struct Line(Coordinate Start, Coordinate End, Direction Direction)
{
    /// <summary>
    /// Determines if the line is horizontal (i.e. if its direction is either East or West)
    /// </summary>
    /// <returns>A boolean value indicating if the line is horiztonal</returns>
    public bool IsHorizontal() => Direction switch
    {
        Direction.East or Direction.West => true,
        _ => false
    };
    
    /// <summary>
    /// Returns the total number of locations on the line
    /// </summary>
    /// <returns>An integer value representing the total number of locations on the line</returns>
    public int GetTotalLocations() => Direction switch
    {
        Direction.North => Start.Y - End.Y,
        Direction.South => End.Y - Start.Y,
        Direction.East => End.X - Start.X,
        Direction.West => Start.X - End.X,
        _ => 0
    };
}