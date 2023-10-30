using Microsoft.Extensions.Logging;
using Moq;
using RobotCleaner.Db;
using RobotCleaner.Models.Shared;

namespace RobotCleaner.Test.Db;

public class PostgresServiceTest
{
    private readonly Mock<IDbClient> _mockDbClient = new();
    private readonly Mock<ILogger<PostgresHandler>> _mockLogger = new();
    private ExecutionReport _executionReport;

    [SetUp]
    public void Setup()
    {
        _executionReport = new ExecutionReport(DateTime.Now, 1, 5, 0.004);
    }

    [Test]
    public async Task TestInsertExecutionReport()
    {
        // Arrange
        var service = new PostgresHandler(_mockDbClient.Object, _mockLogger.Object);
        // Act
        await service.InsertExecutionReport(_executionReport);
        // Assert
        _mockDbClient.Verify(c => c.Insert(It.IsAny<ExecutionReport>()), Times.Once);
    }
}