using Timer = RobotCleaner.Shared.Timer;

namespace RobotCleaner.Test.Shared;

public class TimerTest
{
    [Test]
    public void TestMeasureDuration()
    {
        // Arrange
        var sleep = new Action(() =>
        {
            // Sleep for 1 second
            Thread.Sleep(1000);
        });
        // Act
        var duration = Timer.MeasureDuration(sleep);
        // Assert
        Assert.That(duration, Is.GreaterThanOrEqualTo(1));
    }
}