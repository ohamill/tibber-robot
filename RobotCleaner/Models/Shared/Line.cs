namespace RobotCleaner.Models.Shared;

/// <summary>
/// Represents a 2-D line
/// </summary>
/// <param name="Start">The line's starting coordinates</param>
/// <param name="End">The line's ending coordinates</param>
/// <param name="Direction">The line's direction (valid values are North, South, East, West)</param>
public record struct Line(Coordinate Start, Coordinate End, Direction Direction)
{
    private readonly IDirectionalLine _directionalLine = Direction switch
    {
        Direction.North => new NorthLine(Start, End),
        Direction.South => new SouthLine(Start, End),
        Direction.East => new EastLine(Start, End),
        Direction.West => new WestLine(Start, End)
    };
    /// <summary>
    /// Determines if the line is horizontal (i.e. if its direction is either East or West)
    /// </summary>
    /// <returns>A boolean value indicating if the line is horizontal</returns>
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

    public HashSet<int> CalculateIntersections(Dictionary<int, List<Line>> perpendicularLines) =>
        _directionalLine.CalculateIntersections(perpendicularLines);

    public HashSet<int> CalculateOverlaps(Dictionary<int, List<Line>> parallelLines) =>
        _directionalLine.CalculateOverlaps(parallelLines);
}

/// <summary>
/// Represents a line with only a single possible direction. Whereas the <c>Line</c> type can have one of four directions,
/// an <c>IDirectionalLine</c> will only have one possible direction, which makes it useful when calculating a line's
/// intersections and overlaps.
/// </summary>
public interface IDirectionalLine
{
    /// <summary>
    /// Calculates a set of intersections between the current line and a list of perpendicular lines
    /// </summary>
    /// <param name="perpendicularLines">A dictionary of integers to lists of perpendicular lines. If the current line
    /// is horizontal, the dictionary's integer keys represent columns on a 2-D grid, and if the current line is vertical
    /// then the dictionary's keys represent rows on a 2-D grid.</param>
    /// <returns>A set of integers representing where the current line intersects with existing lines on the path</returns>
    HashSet<int> CalculateIntersections(Dictionary<int, List<Line>> perpendicularLines);
    /// <summary>
    /// Calculates a set of overlapping locations between the current line and a list of parallel lines
    /// </summary>
    /// <param name="parallelLines">A dictionary of integers to lists of parallel lines. If the current line is horizontal,
    /// the dictionary's integer keys represent rows on a 2-D grid, and if the current line is vertical then the dictionary's
    /// keys represent columns on a 2-D grid.</param>
    /// <returns>a set of integers representing where the current line overlaps with existing lines on the path.</returns>
    HashSet<int> CalculateOverlaps(Dictionary<int, List<Line>> parallelLines);
}

public record struct NorthLine(Coordinate Start, Coordinate End) : IDirectionalLine
{
    public HashSet<int> CalculateIntersections(Dictionary<int, List<Line>> perpendicularLines)
    {
        var intersections = new HashSet<int>();
        
        foreach (var (row, lines) in perpendicularLines)
        {
            if (row >= Start.Y || row < End.Y) continue;
            foreach (var line in lines)
            {
                if (line.Direction is Direction.East)
                {
                    if (line.Start.X >= Start.X || line.End.X < Start.X) continue;
                    intersections.Add(row);
                }
                else
                {
                    if (line.Start.X <= Start.X || line.End.X > Start.X) continue;
                    intersections.Add(row);
                }
            }
        }

        return intersections;
    }

    public HashSet<int> CalculateOverlaps(Dictionary<int, List<Line>> parallelLines)
    {
        var overlaps = new HashSet<int>();
        
        if (!parallelLines.TryGetValue(Start.X, out var vertLines)) return overlaps;
        foreach (var vertLine in vertLines)
        {
            if (vertLine.Direction is Direction.North)
            {
                if (Start.Y <= vertLine.Start.Y && End.Y >= vertLine.End.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(End.Y, Start.Y - End.Y));
                }
                else if (Start.Y >= vertLine.Start.Y && End.Y <= vertLine.End.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(vertLine.End.Y, vertLine.Start.Y - vertLine.End.Y));
                }
                else if (Start.Y >= vertLine.Start.Y && End.Y <= vertLine.Start.Y &&
                         End.Y >= vertLine.End.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(End.Y, vertLine.End.Y - End.Y));
                }
                else if (Start.Y <= vertLine.Start.Y && Start.Y >= vertLine.End.Y &&
                         End.Y <= vertLine.End.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(vertLine.End.Y, Start.Y - vertLine.End.Y));
                }
            }
            else
            {
                if (Start.Y >= vertLine.End.Y && End.Y <= vertLine.End.Y &&
                    End.Y >= vertLine.Start.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(End.Y, vertLine.End.Y - End.Y));
                }
                else if (Start.Y >= vertLine.Start.Y && Start.Y <= vertLine.End.Y &&
                         End.Y <= vertLine.Start.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(vertLine.Start.Y, Start.Y - vertLine.Start.Y));
                }
                else if (Start.Y <= vertLine.End.Y && End.Y >= vertLine.Start.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(Start.Y, Start.Y - End.Y));
                }
                else if (Start.Y >= vertLine.End.Y && End.Y <= vertLine.Start.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(vertLine.Start.Y, vertLine.End.Y - vertLine.Start.Y));
                }
            }
        }

        return overlaps;
    }
}

public record struct SouthLine(Coordinate Start, Coordinate End) : IDirectionalLine
{
    public HashSet<int> CalculateIntersections(Dictionary<int, List<Line>> perpendicularLines)
    {
        var intersections = new HashSet<int>();

        foreach (var (row, lines) in perpendicularLines)
        {
            if (row <= Start.Y || row > End.Y) continue;
            foreach (var line in lines)
            {
                if (line.Direction is Direction.East)
                {
                    if (line.Start.X <= Start.X || line.End.X > Start.X) continue;
                    intersections.Add(row);
                }
                else
                {
                    if (line.Start.X >= Start.X || line.End.X < Start.X) continue;
                    intersections.Add(row);
                }
            }
        }

        return intersections;
    }
    
    public HashSet<int> CalculateOverlaps(Dictionary<int, List<Line>> parallelLines)
    {
        var overlaps = new HashSet<int>();
        
        if (!parallelLines.TryGetValue(Start.X, out var vertLines)) return overlaps;
        foreach (var vertLine in vertLines)
        {
            if (vertLine.Direction is Direction.South)
            {
                if (Start.Y >= vertLine.Start.Y && End.Y <= vertLine.End.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(Start.Y, End.Y - Start.Y));
                }
                else if (Start.Y <= vertLine.Start.Y && End.Y >= vertLine.End.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(vertLine.Start.Y, vertLine.End.Y - vertLine.Start.Y));
                }
                else if (Start.Y <= vertLine.Start.Y && End.Y >= vertLine.Start.Y &&
                         End.Y <= vertLine.End.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(vertLine.Start.Y, End.Y - vertLine.Start.Y));
                }
                else if (Start.Y >= vertLine.Start.Y && Start.Y <= vertLine.End.Y &&
                         End.Y >= vertLine.End.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(Start.Y, vertLine.End.Y - Start.Y));
                }
            }
            else
            {
                if (Start.Y <= vertLine.End.Y && End.Y >= vertLine.End.Y &&
                    End.Y <= vertLine.Start.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(vertLine.End.Y, End.Y - vertLine.End.Y));
                }
                else if (Start.Y >= vertLine.End.Y && Start.Y <= vertLine.Start.Y &&
                         End.Y >= vertLine.Start.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(Start.Y, vertLine.Start.Y - Start.Y));
                }
                else if (Start.Y >= vertLine.End.Y && End.Y <= vertLine.Start.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(Start.Y, End.Y - Start.Y));
                }
                else if (Start.Y <= vertLine.Start.Y && End.Y >= vertLine.Start.Y)
                {
                    overlaps.UnionWith(Enumerable.Range(vertLine.End.Y, vertLine.Start.Y - vertLine.End.Y));
                }
            }
        }

        return overlaps;
    }
}

public record struct EastLine(Coordinate Start, Coordinate End) : IDirectionalLine
{
    public HashSet<int> CalculateIntersections(Dictionary<int, List<Line>> perpendicularLines)
    {
        var intersections = new HashSet<int>();

        foreach (var (col, lines) in perpendicularLines)
        {
            if (col <= Start.X || col > End.X) continue;
            foreach (var line in lines)
            {
                if (line.Direction is Direction.North)
                {
                    if (Start.Y >= line.End.Y || Start.Y < line.Start.Y) continue; 
                    intersections.Add(col);
                }
                else
                {
                    if (Start.Y <= line.End.Y || Start.Y > line.Start.Y) continue;
                    intersections.Add(col);
                }
            }
        }

        return intersections;
    }
    
    public HashSet<int> CalculateOverlaps(Dictionary<int, List<Line>> parallelLines)
    {
        var overlaps = new HashSet<int>();
        
        if (!parallelLines.TryGetValue(Start.Y, out var horLines)) return overlaps;
        foreach (var horLine in horLines)
        {
            if (horLine.Direction is Direction.East)
            {
                if (Start.X >= horLine.Start.X && End.X <= horLine.End.X)
                {
                    overlaps.UnionWith(Enumerable.Range(Start.X, End.X - Start.X));
                }
                else if (Start.X <= horLine.Start.X && End.X >= horLine.End.X)
                {
                    overlaps.UnionWith(Enumerable.Range(horLine.Start.X, horLine.End.X - horLine.Start.X));
                }
                else if (Start.X <= horLine.Start.X && End.X >= horLine.Start.X &&
                         End.X <= horLine.End.X)
                {
                    overlaps.UnionWith(Enumerable.Range(horLine.Start.X, End.X - horLine.Start.X));
                }
                else if (Start.X >= horLine.Start.X && Start.X <= horLine.End.X &&
                         End.X >= horLine.End.X)
                {
                    overlaps.UnionWith(Enumerable.Range(Start.X, horLine.End.X - Start.X));
                }
            }
            else
            {
                if (Start.X >= horLine.End.X && End.X <= horLine.Start.X)
                {
                    overlaps.UnionWith(Enumerable.Range(Start.X, End.X - Start.X));
                } 
                else if (Start.X <= horLine.End.X && End.X >= horLine.Start.X)
                {
                    overlaps.UnionWith(Enumerable.Range(horLine.End.X, horLine.Start.X - horLine.End.X));
                }
                else if (Start.X >= horLine.End.X && Start.X <= horLine.Start.X &&
                         End.X >= horLine.Start.X)
                {
                    overlaps.UnionWith(Enumerable.Range(Start.X, horLine.Start.X - Start.X));
                }
                else if (Start.X <= horLine.End.X && End.X >= horLine.End.X &&
                         End.X <= horLine.Start.X)
                {
                    overlaps.UnionWith(Enumerable.Range(horLine.End.X, End.X - horLine.End.X));
                }
            }
        }

        return overlaps;
    }
}

public record struct WestLine(Coordinate Start, Coordinate End) : IDirectionalLine
{
    public HashSet<int> CalculateIntersections(Dictionary<int, List<Line>> perpendicularLines)
    {
        var intersections = new HashSet<int>();

        foreach (var (col, lines) in perpendicularLines)
        {
            if (col >= Start.X || col < End.X) continue;
            foreach (var line in lines)
            {
                if (line.Direction is Direction.North)
                {
                    if (Start.Y <= line.End.Y || Start.Y > line.Start.Y) continue;
                    intersections.Add(col);
                }
                else
                {
                    if (Start.Y >= line.End.Y || Start.Y < line.Start.Y) continue;
                    intersections.Add(col);
                }
            }
        }

        return intersections;
    }
    
    public HashSet<int> CalculateOverlaps(Dictionary<int, List<Line>> parallelLines)
    {
        var overlaps = new HashSet<int>();
        
        if (!parallelLines.TryGetValue(Start.Y, out var horLines)) return overlaps;
        foreach (var horLine in horLines)
        {
            if (horLine.Direction is Direction.West)
            {
                if (Start.X <= horLine.Start.X && End.X >= horLine.End.X)
                {
                    overlaps.UnionWith(Enumerable.Range(End.X, Start.X - End.X));
                }
                else if (Start.X >= horLine.Start.X && End.X <= horLine.End.X)
                {
                    overlaps.UnionWith(Enumerable.Range(horLine.End.X, horLine.Start.X - horLine.End.X));
                } else if (Start.X >= horLine.Start.X && End.X <= horLine.Start.X &&
                           End.X >= horLine.End.X)
                {
                    overlaps.UnionWith(Enumerable.Range(End.X, horLine.Start.X - End.X));
                }
                else if (Start.X <= horLine.Start.X && Start.X >= horLine.End.X &&
                         End.X <= horLine.End.X)
                {
                    overlaps.UnionWith(Enumerable.Range(horLine.End.X, Start.X - horLine.End.X));
                }
            }
            else
            {
                if (Start.X <= horLine.End.X && End.X >= horLine.Start.X)
                {
                    overlaps.UnionWith(Enumerable.Range(End.X, Start.X - End.X));
                }
                else if (Start.X >= horLine.End.X && End.X <= horLine.Start.X)
                {
                    overlaps.UnionWith(Enumerable.Range(horLine.Start.X, horLine.End.X - horLine.Start.X));
                }
                else if (Start.X >= horLine.Start.X && Start.X <= horLine.End.X &&
                         End.X <= horLine.Start.X)
                {
                    overlaps.UnionWith(Enumerable.Range(horLine.Start.X, Start.X - horLine.Start.X));
                }
                if (Start.X >= horLine.End.X && End.X >= horLine.Start.X &&
                    End.X < horLine.End.X)
                {
                    overlaps.UnionWith(Enumerable.Range(End.X, horLine.End.X - End.X));
                }
            }
        }

        return overlaps;
    }
}