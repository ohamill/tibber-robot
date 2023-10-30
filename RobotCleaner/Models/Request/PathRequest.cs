using RobotCleaner.Models.Shared;

namespace RobotCleaner.Models.Request;

public record PathRequest(Coordinate Start, IEnumerable<Command> Commands);