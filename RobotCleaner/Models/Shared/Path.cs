namespace RobotCleaner.Models.Shared;

public class Path
{
    private readonly Dictionary<int, List<Line>> _horizontalLines = new();
    private readonly Dictionary<int, List<Line>> _verticalLines = new();
    private long _uniqueLocationsCleaned = 1; // Account for starting location

    public long GetUniqueLocations() => _uniqueLocationsCleaned;

    /// <summary>
    /// Adds a new line to the path, first comparing it against all other lines currently in the path for intersections
    /// and overlaps.
    /// </summary>
    /// <param name="line">The <c>Line</c> to add to the path</param>
    public void Add(Line line)
    {
        var intersectionsAndOverlaps = CalculateIntersections(line);
        intersectionsAndOverlaps.UnionWith(CalculateOverlaps(line));
        AddLineToDict(line);
        _uniqueLocationsCleaned += line.GetTotalLocations() - intersectionsAndOverlaps.Count;
    }

    /// <summary>
    /// Calculates any intersections between the new line and existing lines in the path
    /// </summary>
    /// <param name="line">The new line to be checked for intersections</param>
    /// <returns>A set of points where the new line intersects with existing lines. If the new line is horizontal, the hash set's
    /// integer values will represent points on the Y-axis, and if the new line is vertical the integer values will represent points
    /// on the X-axis.</returns>
    private HashSet<int> CalculateIntersections(Line line)
    {
        var perpendicularLines = line.IsHorizontal() ? _verticalLines : _horizontalLines;
        return line.CalculateIntersections(perpendicularLines);
    }

    /// <summary>
    /// Calculates any overlap between the new line and existing lines in the path
    /// </summary>
    /// <param name="line">The new line to be checked for overlap</param>
    /// <returns>A set of points where the new line overlaps with existing lines. If the new line is horizontal, the hash set's
    /// integer values will represent points on the X-axis, and if the new line is vertical the integer values will represent
    /// points on the Y-axis.</returns>
    private HashSet<int> CalculateOverlaps(Line line)
    {
        var parallelLines = line.IsHorizontal() ? _horizontalLines : _verticalLines;
        return line.CalculateOverlaps(parallelLines);
    }

    /// <summary>
    /// Adds the supplied line to the path's appropriate dictionary, depending on whether the line is horizontal or vertical.
    /// Since new lines are compared to every other line in the path for intersections and overlaps, this method should be
    /// called after that comparison has taken place so that the supplied line is not compared to itself.
    /// </summary>
    /// <param name="line">The line to add to the appropriate dictionary</param>
    private void AddLineToDict(Line line)
    {
        if (line.IsHorizontal())
        {
            var horLines = _horizontalLines.GetValueOrDefault(line.Start.Y, new List<Line>());
            horLines.Add(line);
            _horizontalLines[line.Start.Y] = horLines;
        }
        else
        {
            var vertLines = _verticalLines.GetValueOrDefault(line.Start.X, new List<Line>());
            vertLines.Add(line);
            _verticalLines[line.Start.X] = vertLines;
        }
    }
}