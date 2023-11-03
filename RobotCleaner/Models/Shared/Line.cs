namespace RobotCleaner.Models.Shared;

public record struct Line(Coordinate Start, Coordinate End, Direction Direction)
{
    public Line FlipDirectionIfNecessary()
    {
        switch (Direction)
        {
            case Direction.South:
                return new Line(End, Start, Shared.Direction.North);
            case Direction.West:
                return new Line(End, Start, Shared.Direction.East);
            default:
                return new Line(Start, End, Direction);
        }
        
    }
    public bool IsHorizontal() => Direction is Direction.East || Direction is Direction.West;
    
    public int GetTotalLocations() => Direction switch
    {
        Direction.North => Start.Y - End.Y,
        Direction.South => End.Y - Start.Y,
        Direction.East => End.X - Start.X,
        Direction.West => Start.X - End.X,
        _ => 0
    };
}