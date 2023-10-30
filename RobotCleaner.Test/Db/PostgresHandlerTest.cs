using Microsoft.Extensions.Logging;
using Moq;
using RobotCleaner.Db;
using RobotCleaner.Models.Shared;

namespace RobotCleaner.Test.Db;

public class PostgresHandlerTest
{
    private const int TestId = 1;
    private const int TestCommands = 2;
    private const int TestResult = 3;
    private const double TestDuration = 0.5;
    private readonly DateTime _testTimestamp = DateTime.Now;
    private readonly Mock<IDbClient> _mockDbClient = new();
    private readonly Mock<ILogger<PostgresHandler>> _mockLogger = new();
    private ExecutionReport _executionReport;

    [SetUp]
    public void Setup()
    {
        _executionReport = new ExecutionReport(DateTime.Now, TestCommands, TestResult, TestDuration);
    }

    [Test]
    public async Task TestInsertExecutionReport()
    {
        // Arrange
        var handler = new PostgresHandler(_mockDbClient.Object, _mockLogger.Object);
        // Act
        await handler.InsertExecutionReport(_executionReport);
        // Assert
        _mockDbClient.Verify(c => c.Insert(It.IsAny<ExecutionReport>()), Times.Once);
    }

    [Test]
    public async Task TestGetExecutionReport()
    {
        // Arrange
        _mockDbClient.Setup(c => c.Get(It.IsAny<int>())).ReturnsAsync(new object[]
        {
            TestId,
            _testTimestamp,
            TestCommands,
            TestResult,
            TestDuration
        });
        var handler = new PostgresHandler(_mockDbClient.Object, _mockLogger.Object);
        // Act
        var executionReport = await handler.GetExecutionReport(TestId);
        // Assert
        _mockDbClient.Verify(c => c.Get(It.IsAny<int>()), Times.Once);
        Assert.Multiple(() =>
        {
            Assert.That(executionReport, Is.Not.Null);
            Assert.That(executionReport!.Id, Is.EqualTo(TestId));
            Assert.That(executionReport.Timestamp, Is.EqualTo(_testTimestamp));
            Assert.That(executionReport.Commands, Is.EqualTo(TestCommands));
            Assert.That(executionReport.Result, Is.EqualTo(TestResult));
            Assert.That(executionReport.Duration, Is.EqualTo(TestDuration));
            
        });
    }
}