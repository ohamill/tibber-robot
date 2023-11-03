namespace RobotCleaner.Models.Shared;

public class Path
{
    private readonly Dictionary<int, List<Line>> _horizontalLines = new();
    private readonly Dictionary<int, List<Line>> _verticalLines = new();
    private int _uniqueLocationsCleaned = 1; // Account for starting location

    public int GetUniqueLocations() => _uniqueLocationsCleaned;

    public void Add(Line line)
    {
        var horizontal = line.IsHorizontal();
        var intersectionsAndOverlaps = CalculateIntersections(line);
        intersectionsAndOverlaps.UnionWith(CalculateOverlaps(line, horizontal));
        AddLineToDict(line, horizontal);
        _uniqueLocationsCleaned += line.GetTotalLocations() - intersectionsAndOverlaps.Count;
    }

    private HashSet<int> CalculateIntersections(Line line)
    {
        var intersections = new HashSet<int>();
        if (line.IsHorizontal())
        {
            foreach (var (col, vertLines) in _verticalLines)
            {
                if (line.Direction == Direction.East)
                {
                    if (col <= line.Start.X || col > line.End.X) continue;
                    foreach (var vertLine in vertLines)
                    {
                        if (vertLine.Direction == Direction.North)
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
                        if (vertLine.Direction == Direction.North)
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
                if (line.Direction == Direction.North)
                {
                    if (row >= line.Start.Y || row < line.End.Y) continue;
                    foreach (var horLine in horLines)
                    {
                        if (horLine.Direction == Direction.East)
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
                        if (horLine.Direction == Direction.East)
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

    private HashSet<int> CalculateOverlaps(Line line, bool horizontal)
    {
        var overlaps = new HashSet<int>();
        if (horizontal)
        {
            if (!_horizontalLines.TryGetValue(line.Start.Y, out var horLines)) return overlaps;
            foreach (var horLine in horLines)
            {
                if (line.Direction == horLine.Direction)
                {
                    if (line.Direction == Direction.East)
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
                    if (line.Direction == Direction.East)
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
                    if (line.Direction == Direction.North)
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
                    if (line.Direction == Direction.North)
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

    private void AddLineToDict(Line line, bool horizontal)
    {
        if (horizontal)
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