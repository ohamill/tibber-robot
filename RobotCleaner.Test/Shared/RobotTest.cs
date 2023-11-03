using RobotCleaner.Models.Shared;
using RobotCleaner.Shared;

namespace RobotCleaner.Test.Shared;

public class RobotTest
{
    [Test]
    public void TestExecuteCommands()
    {
        // Arrange
        var commands = new List<Command>
        {
            new(Direction.North, 4),
            new(Direction.East, 2)
        };
        var robot = new Robot();
        // Act
        var executionReport = robot.ExecuteCommands(new Coordinate(0, 0), commands);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(executionReport.Result, Is.EqualTo(7));
            Assert.That(executionReport.Commands, Is.EqualTo(2));
            Assert.That(executionReport.Duration, Is.GreaterThan(0));
        });
    }

    [Test]
    public void TestExecuteCommandsWithDuplicates_NewLineIsAllDuplicates()
    {
        // Arrange
        var commands = new List<Command>
        {
            new(Direction.North, 4),
            // Backtrack over locations that have already been visited. These locations should not be counted twice.
            new(Direction.South, 2)
        };
        var robot = new Robot();
        // Act
        var executionReport = robot.ExecuteCommands(new Coordinate(0, 0), commands);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(executionReport.Result, Is.EqualTo(5));
            Assert.That(executionReport.Commands, Is.EqualTo(2));
            Assert.That(executionReport.Duration, Is.GreaterThan(0));
        });
    }

    [Test]
    public void TestExecuteCommandsWithDuplicates_ExistingLineIsAllDuplicates()
    {
        // Arrange
        var commands = new List<Command>
        {
            new(Direction.North, 4),
            // Backtrack over locations that have already been visited, but also visit new locations. The duplicate locations should not be counted
            // but the new locations should be.
            new(Direction.South, 10)
        };
        var robot = new Robot();
        // Act
        var executionReport = robot.ExecuteCommands(new Coordinate(0, 0), commands);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(executionReport.Result, Is.EqualTo(11));
            Assert.That(executionReport.Commands, Is.EqualTo(2));
            Assert.That(executionReport.Duration, Is.GreaterThan(0));
        });
    }

    [Test]
    public void TestExecuteCommandsWithDuplicates_NewLineStartsInNewLocationButEndsInDuplicateLocation()
    {
        // Arrange
        var commands = new List<Command>
        {
            new(Direction.North, 4),
            new(Direction.East, 2),
            new(Direction.North, 4),
            new(Direction.West, 2),
            // Start traversing new locations, but then overlap with previous line
            new(Direction.South, 6)
        };
        var robot = new Robot();
        // Act
        var executionReport = robot.ExecuteCommands(new Coordinate(0, 0), commands);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(executionReport.Result, Is.EqualTo(17));
            Assert.That(executionReport.Commands, Is.EqualTo(5));
            Assert.That(executionReport.Duration, Is.GreaterThan(0));
        });
    }

    [Test]
    public void TestExecuteCommandsWithDuplicates_NewLineStartsInDuplicateLocationButEndsInNewLocation()
    {
        // Arrange
        var commands = new List<Command>
        {
            new(Direction.North, 4),
            new(Direction.West, 2),
            new(Direction.South, 2),
            new(Direction.East, 2),
            // Start traversing duplicate locations, but then traverse new locations
            new(Direction.South, 4)
        };
        var robot = new Robot();
        // Act
        var executionReport = robot.ExecuteCommands(new Coordinate(0, 0), commands);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(executionReport.Result, Is.EqualTo(13));
            Assert.That(executionReport.Commands, Is.EqualTo(5));
            Assert.That(executionReport.Duration, Is.GreaterThan(0));
        });
    }

    [Test]
    public void TestExecuteCommandsWithDuplicates_SingleIntersection()
    {
        // Arrange
        var commands = new List<Command>
        {
            new(Direction.North, 4),
            new(Direction.East, 4),
            new(Direction.South, 2),
            // This line will intersect with the first line in the path. The one duplicate location should only be counted once.
            new(Direction.West, 10)
        };
        var robot = new Robot();
        // Act
        var executionReport = robot.ExecuteCommands(new Coordinate(0, 0), commands);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(executionReport.Result, Is.EqualTo(20));
            Assert.That(executionReport.Commands, Is.EqualTo(4));
            Assert.That(executionReport.Duration, Is.GreaterThan(0));
        });
    }

    [Test]
    public void TestExecuteCommandsWithDuplicates_DoubleIntersection()
    {
        // Arrange
        var commands = new List<Command>
        {
            new(Direction.North, 4),
            new(Direction.East, 4),
            new(Direction.South, 4),
            new(Direction.East, 4),
            new(Direction.North, 2),
            // This line will intersect with the first and third lines in the path. The two duplicate locations should each only be counted once.
            new(Direction.West, 10)
        };
        var robot = new Robot();
        // Act
        var executionReport = robot.ExecuteCommands(new Coordinate(0, 0), commands);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(executionReport.Result, Is.EqualTo(27));
            Assert.That(executionReport.Commands, Is.EqualTo(6));
            Assert.That(executionReport.Duration, Is.GreaterThan(0));
        });
    }

    [Test]
    public void TestExecuteCommandsWithNonZeroStartingPosition()
    {
        // Arrange
        var commands = new List<Command>
        {
            new(Direction.North, 4)
        };
        var robot = new Robot();
        // Act
        var executionReport = robot.ExecuteCommands(new Coordinate(5, 7), commands);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(executionReport.Result, Is.EqualTo(5));
            Assert.That(executionReport.Commands, Is.EqualTo(1));
            Assert.That(executionReport.Duration, Is.GreaterThan(0));
        });
    }

    [Test]
    public void TestExecuteCommandsWithZeroCommands()
    {
        // Arrange
        var robot = new Robot();
        // Act
        var executionReport = robot.ExecuteCommands(new Coordinate(0, 0), new List<Command>());
        // Assert
        Assert.Multiple(() =>
        {
            // Result should be 1 because the robot will still clean its starting position
            Assert.That(executionReport.Result, Is.EqualTo(1));
            Assert.That(executionReport.Commands, Is.EqualTo(0));
            Assert.That(executionReport.Duration, Is.GreaterThanOrEqualTo(0));
        });
    }
}