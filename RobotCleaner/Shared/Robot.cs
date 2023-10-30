using RobotCleaner.Models.Shared;

namespace RobotCleaner.Shared;

public interface IRobot
{
    /// <summary>
    /// Executes a series of commands
    /// </summary>
    /// <param name="start">The robot's starting location. This is where the robot will be positioned when it begins
    /// executing the commands</param>
    /// <param name="commands">The collection of commands to execute</param>
    /// <returns>An ExecutionReport object providing information about the robot's execution, including how many commands
    /// were processed, how long it took to process those commands, and how many locations were cleaned</returns>
    ExecutionReport ExecuteCommands(Coordinate start, IEnumerable<Command> commands);
}

public class Robot : IRobot
{
    private Coordinate _currentLocation;
    private readonly HashSet<Coordinate> _uniquePlacesCleaned = new();
    
    public ExecutionReport ExecuteCommands(Coordinate start, IEnumerable<Command> commands)
    {
        var duration = Timer.MeasureDuration(() =>
        {
            // Place robot at starting location
            _currentLocation = start;
            _uniquePlacesCleaned.Add(_currentLocation);

            foreach (var command in commands)
            {
                ExecuteCommand(command);
            }
        });

        return new ExecutionReport(DateTime.Now, commands.Count(), _uniquePlacesCleaned.Count, duration);
    }

    /// <summary>
    /// Executes a single command, which will direct the robot to move a certain number of squares in a specific direction
    /// along a 2-D axis.
    /// </summary>
    /// <param name="command">The command to execute</param>
    private void ExecuteCommand(Command command)
    {
        for (var i = 0; i < command.Steps; i++)
        {
            _currentLocation = UpdateCoordinate(command.Direction);
            _uniquePlacesCleaned.Add(_currentLocation);
        }
    }

    /// <summary>
    /// Moves the robot one square in the supplied direction and updates its current location accordingly
    /// </summary>
    /// <param name="direction">The direction in which the robot will move. This direction will determine how the robot's coordinate will
    /// be updated (i.e. moving North or South will affect the robot's Y coordinate, and moving East or West will affect its X coordinate.</param>
    /// <returns>A Coordinate value containing the robot's new current position</returns>
    private Coordinate UpdateCoordinate(Direction direction) => direction switch
    {
        Direction.North => _currentLocation with { Y = _currentLocation.Y + 1 },
        Direction.East => _currentLocation with { X = _currentLocation.X + 1 },
        Direction.South => _currentLocation with { Y = _currentLocation.Y - 1 },
        Direction.West => _currentLocation with { X = _currentLocation.X - 1 },
        _ => _currentLocation
    };
}