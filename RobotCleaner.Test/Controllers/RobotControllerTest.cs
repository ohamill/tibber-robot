using Microsoft.Extensions.Logging;
using Moq;
using RobotCleaner.Controllers;
using RobotCleaner.Models.Request;
using RobotCleaner.Models.Shared;
using RobotCleaner.Db;
using RobotCleaner.Shared;

namespace RobotCleaner.Test.Controllers;

public class RobotControllerTest
{
    private const int TestId = 1;
    private const int TestCommands = 2;
    private const int TestResult = 3;
    private const double TestDuration = 0.1;
    private readonly Mock<ILogger<RobotController>> _mockLogger = new();
    private readonly Mock<IRobot> _mockRobot = new();
    private readonly Mock<IDbHandler> _mockDbService = new();

    [SetUp]
    public void Setup()
    {
        _mockDbService.Setup(db => db.InsertExecutionReport(It.IsAny<ExecutionReport>())).ReturnsAsync(TestId);
        _mockRobot.Setup(r => r.ExecuteCommands(It.IsAny<Coordinate>(), It.IsAny<IEnumerable<Command>>())).Returns(() => new ExecutionReport(DateTime.Now, TestCommands, TestResult, TestDuration));
    }

    [Test]
    public async Task TestPostPath()
    {
        // Arrange
        var pathRequest = new PathRequest(new Coordinate(0, 0), new List<Command>
        {
            new(Direction.North, 2),
            new(Direction.East, 4)
        });
        var controller = new RobotController(_mockLogger.Object, _mockRobot.Object, _mockDbService.Object);
        // Act
        var response = await controller.PostPath(pathRequest);
        // Assert
        _mockDbService.Verify(s => s.InsertExecutionReport(It.IsAny<ExecutionReport>()), Times.Once);

        var pathResponse = response.Value!;
        Assert.Multiple(() =>
        {
            Assert.That(pathResponse.Id, Is.EqualTo(TestId));
            Assert.That(pathResponse.Commands, Is.EqualTo(TestCommands));
            Assert.That(pathResponse.Result, Is.EqualTo(TestResult));
        });
    }
}