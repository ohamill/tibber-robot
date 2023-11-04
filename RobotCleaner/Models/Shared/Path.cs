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
        var intersections = new HashSet<int>();
        if (line.IsHorizontal())
        {
            foreach (var (col, vertLines) in _verticalLines)
            {
                if (line.Direction is Direction.East)
                {
                    if (col <= line.Start.X || col > line.End.X) continue;
                    foreach (var vertLine in vertLines)
                    {
                        if (vertLine.Direction is Direction.North)
                        {
                            if (line.Start.Y >= vertLine.End.Y || line.Start.Y < vertLine.Start.Y) continue; 
                            intersections.Add(col);
                        }
                        else
                        {
                            if (line.Start.Y <= vertLine.End.Y || line.Start.Y > vertLine.Start.Y) continue;
                            intersections.Add(col);
                        }
                    }
                }
                else
                {
                    if (col >= line.Start.X || col < line.End.X) continue;
                    foreach (var vertLine in vertLines)
                    {
                        if (vertLine.Direction is Direction.North)
                        {
                            if (line.Start.Y <= vertLine.End.Y || line.Start.Y > vertLine.Start.Y) continue;
                            intersections.Add(col);
                        }
                        else
                        {
                            if (line.Start.Y >= vertLine.End.Y || line.Start.Y < vertLine.Start.Y) continue;
                            intersections.Add(col);
                        }
                    }
                }
            }
        }
        else
        {
            foreach (var (row, horLines) in _horizontalLines)
            {
                if (line.Direction is Direction.North)
                {
                    if (row >= line.Start.Y || row < line.End.Y) continue;
                    foreach (var horLine in horLines)
                    {
                        if (horLine.Direction is Direction.East)
                        {
                            if (horLine.Start.X >= line.Start.X || horLine.End.X < line.Start.X) continue;
                            intersections.Add(row);
                        }
                        else
                        {
                            if (horLine.Start.X <= line.Start.X || horLine.End.X > line.Start.X) continue;
                            intersections.Add(row);
                        }
                    }
                }
                else
                {
                    if (row <= line.Start.Y || row > line.End.Y) continue;
                    foreach (var horLine in horLines)
                    {
                        if (horLine.Direction is Direction.East)
                        {
                            if (horLine.Start.X <= line.Start.X || horLine.End.X > line.Start.X) continue;
                            intersections.Add(row);
                        }
                        else
                        {
                            if (horLine.Start.X >= line.Start.X || horLine.End.X < line.Start.X) continue;
                            intersections.Add(row);
                        }
                    }
                }
            }
        }

        return intersections;
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
        var overlaps = new HashSet<int>();
        if (line.IsHorizontal())
        {
            if (!_horizontalLines.TryGetValue(line.Start.Y, out var horLines)) return overlaps;
            foreach (var horLine in horLines)
            {
                if (line.Direction == horLine.Direction)
                {
                    if (line.Direction is Direction.East)
                    {
                        if (line.Start.X >= horLine.Start.X && line.End.X <= horLine.End.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.Start.X, line.End.X - line.Start.X));
                        }
                        else if (line.Start.X <= horLine.Start.X && line.End.X >= horLine.End.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(horLine.Start.X, horLine.End.X - horLine.Start.X));
                        }
                        else if (line.Start.X <= horLine.Start.X && line.End.X >= horLine.Start.X &&
                                 line.End.X <= horLine.End.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(horLine.Start.X, line.End.X - horLine.Start.X));
                        }
                        else if (line.Start.X >= horLine.Start.X && line.Start.X <= horLine.End.X &&
                                 line.End.X >= horLine.End.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.Start.X, horLine.End.X - line.Start.X));
                        }
                    }
                    else
                    {
                        if (line.Start.X <= horLine.Start.X && line.End.X >= horLine.End.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.End.X, line.Start.X - line.End.X));
                        }
                        else if (line.Start.X >= horLine.Start.X && line.End.X <= horLine.End.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(horLine.End.X, horLine.Start.X - horLine.End.X));
                        } else if (line.Start.X >= horLine.Start.X && line.End.X <= horLine.Start.X &&
                                   line.End.X >= horLine.End.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.End.X, horLine.Start.X - line.End.X));
                        }
                        else if (line.Start.X <= horLine.Start.X && line.Start.X >= horLine.End.X &&
                                 line.End.X <= horLine.End.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(horLine.End.X, line.Start.X - horLine.End.X));
                        }
                    }
                }
                else
                {
                    if (line.Direction is Direction.East)
                    {
                        if (line.Start.X >= horLine.End.X && line.End.X <= horLine.Start.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.Start.X, line.End.X - line.Start.X));
                        } 
                        else if (line.Start.X <= horLine.End.X && line.End.X >= horLine.Start.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(horLine.End.X, horLine.Start.X - horLine.End.X));
                        }
                        else if (line.Start.X >= horLine.End.X && line.Start.X <= horLine.Start.X &&
                                 line.End.X >= horLine.Start.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.Start.X, horLine.Start.X - line.Start.X));
                        }
                        else if (line.Start.X <= horLine.End.X && line.End.X >= horLine.End.X &&
                                 line.End.X <= horLine.Start.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(horLine.End.X, line.End.X - horLine.End.X));
                        }
                    }
                    else
                    {
                        if (line.Start.X <= horLine.End.X && line.End.X >= horLine.Start.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.End.X, line.Start.X - line.End.X));
                        }
                        else if (line.Start.X >= horLine.End.X && line.End.X <= horLine.Start.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(horLine.Start.X, horLine.End.X - horLine.Start.X));
                        }
                        else if (line.Start.X >= horLine.Start.X && line.Start.X <= horLine.End.X &&
                                 line.End.X <= horLine.Start.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(horLine.Start.X, line.Start.X - horLine.Start.X));
                        }
                        if (line.Start.X >= horLine.End.X && line.End.X >= horLine.Start.X &&
                            line.End.X < horLine.End.X)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.End.X, horLine.End.X - line.End.X));
                        }
                    }
                }
            }
        }
        else
        {
            if (!_verticalLines.TryGetValue(line.Start.X, out var vertLines)) return overlaps;
            foreach (var vertLine in vertLines)
            {
                if (line.Direction == vertLine.Direction)
                {
                    if (line.Direction is Direction.North)
                    {
                        if (line.Start.Y <= vertLine.Start.Y && line.End.Y >= vertLine.End.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.End.Y, line.Start.Y - line.End.Y));
                        }
                        else if (line.Start.Y >= vertLine.Start.Y && line.End.Y <= vertLine.End.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(vertLine.End.Y, vertLine.Start.Y - vertLine.End.Y));
                        }
                        else if (line.Start.Y >= vertLine.Start.Y && line.End.Y <= vertLine.Start.Y &&
                                 line.End.Y >= vertLine.End.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.End.Y, vertLine.End.Y - line.End.Y));
                        }
                        else if (line.Start.Y <= vertLine.Start.Y && line.Start.Y >= vertLine.End.Y &&
                                 line.End.Y <= vertLine.End.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(vertLine.End.Y, line.Start.Y - vertLine.End.Y));
                        }
                    }
                    else
                    {
                        if (line.Start.Y >= vertLine.Start.Y && line.End.Y <= vertLine.End.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.Start.Y, line.End.Y - line.Start.Y));
                        }
                        else if (line.Start.Y <= vertLine.Start.Y && line.End.Y >= vertLine.End.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(vertLine.Start.Y, vertLine.End.Y - vertLine.Start.Y));
                        }
                        else if (line.Start.Y <= vertLine.Start.Y && line.End.Y >= vertLine.Start.Y &&
                                 line.End.Y <= vertLine.End.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(vertLine.Start.Y, line.End.Y - vertLine.Start.Y));
                        }
                        else if (line.Start.Y >= vertLine.Start.Y && line.Start.Y <= vertLine.End.Y &&
                                 line.End.Y >= vertLine.End.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.Start.Y, vertLine.End.Y - line.Start.Y));
                        }
                    }
                }
                else
                {
                    if (line.Direction is Direction.North)
                    {
                        if (line.Start.Y >= vertLine.End.Y && line.End.Y <= vertLine.End.Y &&
                            line.End.Y >= vertLine.Start.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.End.Y, vertLine.End.Y - line.End.Y));
                        }
                        else if (line.Start.Y >= vertLine.Start.Y && line.Start.Y <= vertLine.End.Y &&
                                 line.End.Y <= vertLine.Start.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(vertLine.Start.Y, line.Start.Y - vertLine.Start.Y));
                        }
                        else if (line.Start.Y <= vertLine.End.Y && line.End.Y >= vertLine.Start.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.Start.Y, line.Start.Y - line.End.Y));
                        }
                        else if (line.Start.Y >= vertLine.End.Y && line.End.Y <= vertLine.Start.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(vertLine.Start.Y, vertLine.End.Y - vertLine.Start.Y));
                        }
                    }
                    else
                    {
                        if (line.Start.Y <= vertLine.End.Y && line.End.Y >= vertLine.End.Y &&
                                                         line.End.Y <= vertLine.Start.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(vertLine.End.Y, line.End.Y - vertLine.End.Y));
                        }
                        else if (line.Start.Y >= vertLine.End.Y && line.Start.Y <= vertLine.Start.Y &&
                                                         line.End.Y >= vertLine.Start.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.Start.Y, vertLine.Start.Y - line.Start.Y));
                        }
                        else if (line.Start.Y >= vertLine.End.Y && line.End.Y <= vertLine.Start.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(line.Start.Y, line.End.Y - line.Start.Y));
                        }
                        else if (line.Start.Y <= vertLine.Start.Y && line.End.Y >= vertLine.Start.Y)
                        {
                            overlaps.UnionWith(Enumerable.Range(vertLine.End.Y, vertLine.Start.Y - vertLine.End.Y));
                        }
                    }
                }
            }
        }

        return overlaps;
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