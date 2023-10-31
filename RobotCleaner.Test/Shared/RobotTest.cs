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
    public void TestExecuteCommandsWithDuplicates()
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